using CacheServices;
using DatabaseServices;
using DatabaseServices.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NutriQuestServices.FoodServices.Enums;
using NutriQuestServices.FoodServices.Projections;
using NutriQuestServices.FoodServices.Requests;
using NutriQuestServices.FoodServices.Responses;
using System.Text.RegularExpressions;

namespace NutriQuestServices.FoodServices;

public class FoodService
{
    private readonly DatabaseService<FoodItem> _dbService;

    private readonly CacheService _cache;

    private readonly string _idsShownKey = "idsShown";

    private readonly int _itemsPerPage = 9;

    private readonly string _imageBaseUrl = "https://images.openfoodfacts.org/images/products";

    private readonly string _frontImageName = "front_en";

    private readonly string _nutritionImageName = "nutrition_en";

    private readonly string _ingredientsImageName = "ingredients_en";

    private readonly string _packagingImageName = "packaging_en";

    private readonly string _barcodeSplitPattern = @"^(...)(...)(...)(.*)$";

    private readonly string _categoryEnumsNamespace;

    public FoodService(DatabaseService<FoodItem> databaseService, CacheService cache)
    {
        _dbService = databaseService;
        _cache = cache;

        // Set the namespace of the food enums so we can ensure we can find the sub categories w/ reflection
        _categoryEnumsNamespace = typeof(MainFoodCategories).Namespace ?? "";
    }

    // **********************
    // Used by the controller
    // **********************

    public async Task<FoodItemByIdResponse> GetFoodItemByIdAsync(FoodItemByIdRequest request)
    {
        var response = new FoodItemByIdResponse();

        var findOptions = new FindOptions<FoodItem>
        {
            Projection = Builders<FoodItem>.Projection.Exclude(x => x.Images)
                                                      .Exclude(x => x.MaxImgId)
                                                      .Exclude(x => x.Rev)
        };
        var filter = Builders<FoodItem>.Filter.Eq(x => x.Id, request.ItemId);

        response.FoodItem = await _dbService.FindOneAsync(filter, findOptions).ConfigureAwait(false);

        return response;
    }

    public async Task<FoodItemFrontImgResponse> GetFoodItemFrontImgUrlAsync(FoodImageRequest request)
    {
        var response = new FoodItemFrontImgResponse();

        var imageFilter = Builders<FoodItem>.Filter.Eq(x => x.Id, request.ItemId);
        var findOptions = new FindOptions<FoodItem, FoodItemImageProjection>
        {
            Projection = Builders<FoodItem>.Projection.Expression(x =>
                new FoodItemImageProjection
                {
                    Id = x.Id,
                    Images = x.Images ?? new List<Image>(),
                    Code = x.Code ?? string.Empty,
                    Rev = x.Rev ?? 0,
                }
            )
        };
        var item = await _dbService.FindOneAsync(imageFilter, findOptions).ConfigureAwait(false);
        if (item == null)
            return response;

        response.Url = BuildImageUrl(item, ImageType.Front);

        return response;
    }

    public async Task<FoodItemAllImgResponse?> GetFoodItemAllImgUrlsAsync(FoodImageRequest request)
    {
        var response = new FoodItemAllImgResponse();

        var imageFilter = Builders<FoodItem>.Filter.Eq(x => x.Id, request.ItemId);
        var findOptions = new FindOptions<FoodItem, FoodItemImageProjection>
        {
            Projection = Builders<FoodItem>.Projection.Expression(x =>
                new FoodItemImageProjection
                {
                    Id = x.Id,
                    Images = x.Images ?? new List<Image>(),
                    Code = x.Code ?? string.Empty,
                    Rev = x.Rev ?? 0,
                }
            )
        };
        var item = await _dbService.FindOneAsync(imageFilter, findOptions).ConfigureAwait(false);
        if (item == null)
            return null;

        var imageTypes = item.Images.Select(x => x.ImageType);
        if (imageTypes == null)
            return null;

        foreach (var type in imageTypes)
        {
            var url = BuildImageUrl(item, (ImageType)type!);
            if (string.IsNullOrEmpty(url))
                continue;

            var imageDetails = new ImageDetails
            {
                Url = url,
                ImageType = type.ToString()!
            };
            response.Images.Add(imageDetails);
        }

        return response;
    }

    private string BuildImageUrl(FoodItemImageProjection item, ImageType type)
    {
        var rev = item.Images.Where(x => x.ImageType == type).FirstOrDefault()?.Rev;
        if (rev == null)
            return string.Empty;

        var barcode = item.Code.PadLeft(13, '0');
        var splitMatch = Regex.Match(barcode, _barcodeSplitPattern);

        string imageName = string.Empty;
        switch (type)
        {
            case ImageType.Front:
                imageName = _frontImageName;
                break;
            case ImageType.Nutrition:
                imageName = _nutritionImageName;
                break;
            case ImageType.Ingredients:
                imageName = _ingredientsImageName;
                break;
            case ImageType.Packaging:
                imageName = _packagingImageName;
                break;
        }

        var folderName = $"{splitMatch.Groups[1].Value}/{splitMatch.Groups[2].Value}/{splitMatch.Groups[3].Value}/{splitMatch.Groups[4].Value}";
        var fileName = $"{imageName}.{rev}.400.jpg";

        return $"{_imageBaseUrl}/{folderName}/{fileName}";
    }

    // TODO: Sorting
    public async Task<List<FoodItemPreviewsResponse>> GetFoodItemPreviewsAsync(FoodItemPreviewsRequest request)
    {
        var sessionId = request.SessionId;
        var prevPage = request.PrevPage;
        var mainCategory = request.Filters.MainCategory;
        var subCategory = request.Filters.SubCategory;
        var restrictions = request.Filters.Restrictions;
        var excludedIngredients = request.Filters.ExcludedIngredients;
        var excludedCustomIngredients = request.Filters.ExcludedCustomIngredients;

        var findOptions = new FindOptions<FoodItem, FoodItemPreviewsResponse>
        {
            Limit = _itemsPerPage,
            Projection = Builders<FoodItem>.Projection.Expression(x =>
                new FoodItemPreviewsResponse
                {
                    Id = x.Id,
                    Name = x.ProductName,
                    Price = x.Price,
                    StoresInStock = x.StoresInStock,
                    Brands = x.Brands,
                    Rating = x.Rating
                }
            )
        };

        List<string> idsShown = [];
        if (request.RestartPaging)
        {
            await _cache.DeleteCacheValue(sessionId);
        }
        else
        {
            var idsShownValue = await _cache.GetCacheValue($"{_idsShownKey}-{sessionId}").ConfigureAwait(false);
            if (!string.IsNullOrEmpty(idsShownValue))
                idsShown = [.. idsShownValue.Split(',')];
        }

        List<FilterDefinition<FoodItem>> filters = [];

        // Category Filtering
        if (mainCategory != null)
        {
            var mainCategoryRegex = CategoryEnumHelper.GetMainFoodCategoryRegex(mainCategory);
            if (!string.IsNullOrEmpty(mainCategoryRegex))
                filters.Add(Builders<FoodItem>.Filter.Regex(x => x.Categories, new BsonRegularExpression(mainCategoryRegex, "i")));

            if (subCategory != null)
            {
                var subCategoryRegex = CategoryEnumHelper.GetSubCategoryRegex(mainCategory, subCategory);
                if (!string.IsNullOrEmpty(subCategoryRegex))
                    filters.Add(Builders<FoodItem>.Filter.Regex(x => x.Categories, new BsonRegularExpression(subCategoryRegex, "i")));
            }
        }

        // Ingredient Filtering
        if (restrictions != null)
        {
            foreach (var restriction in restrictions)
            {
                var restrictionRegex = IngredientEnumHelper.GetFoodRestrictionRegex(restriction);
                filters.Add(
                    Builders<FoodItem>.Filter.And(
                        Builders<FoodItem>.Filter.Not(Builders<FoodItem>.Filter.Regex(x => x.IngredientsText, new BsonRegularExpression(restrictionRegex, "i"))),
                        Builders<FoodItem>.Filter.Not(Builders<FoodItem>.Filter.Regex(x => x.ProductName, new BsonRegularExpression(restrictionRegex, "i")))
                    )
                );
            }
        }

        if (excludedIngredients != null)
        {
            foreach (var ingredient in excludedIngredients)
            {
                var ingredientRegex = IngredientEnumHelper.GetIngredientRegex(ingredient);
                filters.Add(
                    Builders<FoodItem>.Filter.And(
                        Builders<FoodItem>.Filter.Not(Builders<FoodItem>.Filter.Regex(x => x.IngredientsText, new BsonRegularExpression(ingredientRegex, "i"))),
                        Builders<FoodItem>.Filter.Not(Builders<FoodItem>.Filter.Regex(x => x.ProductName, new BsonRegularExpression(ingredientRegex, "i")))
                    )
                );
            }
        }

        if (excludedCustomIngredients != null)
        {
            foreach (var customIngredient in excludedCustomIngredients)
            {
                var escapedIngredient = Regex.Escape(customIngredient.Trim());
                var customIngredientRegex = $"\\b{escapedIngredient}s?\\b";
                filters.Add(
                    Builders<FoodItem>.Filter.And(
                        Builders<FoodItem>.Filter.Not(Builders<FoodItem>.Filter.Regex(x => x.IngredientsText, new BsonRegularExpression(customIngredientRegex, "i"))),
                        Builders<FoodItem>.Filter.Not(Builders<FoodItem>.Filter.Regex(x => x.ProductName, new BsonRegularExpression(customIngredientRegex, "i")))
                    )
                );
            }
        }

        // Pagination
        if (prevPage && idsShown.Count > 1)
        {
            if (idsShown.Count < 3)
            {
                idsShown.RemoveAt(1);
            }
            else
            {
                var prevLastIdShown = idsShown[^3];
                idsShown.RemoveAt(idsShown.Count - 1);
                filters.Add(Builders<FoodItem>.Filter.Gt(x => x.Id, prevLastIdShown));
            }
        }
        else if (idsShown.Count > 0 && !prevPage)
        {
            filters.Add(Builders<FoodItem>.Filter.Gt(x => x.Id, idsShown.Last()));
        }

        var finalFilter = filters.Count > 0 ? Builders<FoodItem>.Filter.And(filters)
                                            : Builders<FoodItem>.Filter.Empty;

        var foodItems = await _dbService.FindAsync(finalFilter, findOptions).ConfigureAwait(false);
        if (foodItems.Count == 0)
            return [];

        if (!prevPage)
            idsShown.Add(foodItems.Last().Id!);

        await _cache.SetCacheValue($"{_idsShownKey}-{sessionId}", string.Join(',', idsShown), 30).ConfigureAwait(false);

        return foodItems;
    }

    public MainFoodCategoriesResponse GetMainFoodCategories()
    {
        var response = new MainFoodCategoriesResponse
        {
            MainFoodCategories = [.. Enum.GetNames(typeof(MainFoodCategories))]
        };

        return response;
    }

    public SubCategoriesResponse GetSubCategoriesForCategory(SubCategoriesRequest request)
    {
        var response = new SubCategoriesResponse();
        var enumType = Type.GetType($"{_categoryEnumsNamespace}.{ request.MainCategory}");
        if (enumType != null)
            response.SubCategories = [.. Enum.GetNames(enumType)];

        return response;
    }

    public IngredientsResponse GetFoodIngredients()
    {
        var response = new IngredientsResponse()
        {
            Ingredients = [.. Enum.GetNames(typeof(Ingredients))]
        };

        return response;
    }

    public FoodRestrictionsResponse GetFoodRestrictions()
    {
        var response = new FoodRestrictionsResponse()
        {
            FoodRestrictions = [.. Enum.GetNames(typeof(FoodRestrictions))]
        };

        return response;
    }

    // TODO: Add AddRatingToItem()
    // TODO: Add GetRatingInfo()

    // **********************
    // Used by other services
    // **********************

    public async Task<List<FoodItemPreviewsResponse>> GetFoodItemPreviewsByIdsAsync(List<string> ids)
    {
        if (ids.Count < 1)
            return [];

        var findFilter = Builders<FoodItem>.Filter.In(x => x.Id, ids);
        var findOptions = new FindOptions<FoodItem, FoodItemPreviewsResponse>
        {
            Limit = _itemsPerPage,
            Projection = Builders<FoodItem>.Projection.Expression(x =>
                new FoodItemPreviewsResponse
                {
                    Id = x.Id,
                    Name = x.ProductName,
                    Price = x.Price,
                    StoresInStock = x.StoresInStock,
                    Brands = x.Brands,
                    Rating = x.Rating
                }
           )
        };

        return await _dbService.FindAsync(findFilter, findOptions).ConfigureAwait(false);
    }
}