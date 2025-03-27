using CacheServices;
using DatabaseServices;
using DatabaseServices.Models;
using DatabaseServices.Responses;
using MongoDB.Bson;
using MongoDB.Driver;
using NutriQuestRepositories.ProductRepo.Enums;
using NutriQuestRepositories.ProductRepo.Responses;
using System.Text.RegularExpressions;

namespace NutriQuestRepositories.ProductRepo;

public class ProductRepository
{
	private readonly DatabaseService<Product> _dbService;

    private readonly CacheService _cache;

    private readonly string _idsShownKey = "idsShown";

    private readonly int _itemsPerPage = 9;

    private readonly string _imageBaseUrl = "https://images.openfoodfacts.org/images/products";

    private readonly string _frontImageName = "front_en";

    private readonly string _nutritionImageName = "nutrition_en";

    private readonly string _ingredientsImageName = "ingredients_en";

    private readonly string _packagingImageName = "packaging_en";

    private readonly string _barcodeSplitPattern = @"^(...)(...)(...)(.*)$";

    public ProductRepository(DatabaseService<Product> dbService, CacheService cache)
	{
		_dbService = dbService;
        _cache = cache;
	}

    public async Task<Product?> GetProductByIdAsync(string id, bool buildFrontImgUrl = false)
    {
        var filter = Builders<Product>.Filter.Eq(x => x.Id, id);
        var product = await _dbService.FindOneAsync(filter).ConfigureAwait(false);

        if (buildFrontImgUrl && product != null)
        {
            product.ImageUrl = BuildImageUrl(product.Images!, product.Code!, ImageType.Front);
        }

        return product;
    }

    // TODO: Sorting
    public async Task<List<ProductPreviewsResponse>> GetProductPreviewsPagingAsync(string sessionId, bool prevPage, bool restartPaging, string? mainCategory, string? subCategory, List<string>? restrictions, List<string>? excludedIngredients, List<string>? excludedCustomIngredients, string? sort)
    {
        var findOptions = new FindOptions<Product, ProductPreviewsResponse>
        {
            Limit = _itemsPerPage,
            Projection = Builders<Product>.Projection.Expression(x =>
                new ProductPreviewsResponse
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
        if (sort != null)
        {
            var sortDefinition = Builders<Product>.Sort.Ascending(x => x.Id);
            if (sort.Contains("Descending"))
            {
                 
            }
            else
            {
                
            }

            
            findOptions.Sort = sortDefinition;
        }

        List<string> idsShown = [];
        if (restartPaging)
        {
            await _cache.DeleteCacheValue(sessionId);
        }
        else
        {
            var idsShownValue = await _cache.GetCacheValue($"{_idsShownKey}-{sessionId}").ConfigureAwait(false);
            if (!string.IsNullOrEmpty(idsShownValue))
                idsShown = [.. idsShownValue.Split(',')];
        }

        List<FilterDefinition<Product>> filters = [];

        // Category Filtering
        if (mainCategory != null)
        {
            var mainCategoryRegex = CategoryEnumHelper.GetMainFoodCategoryRegex(mainCategory);
            if (!string.IsNullOrEmpty(mainCategoryRegex))
                filters.Add(Builders<Product>.Filter.Regex(x => x.Categories, new BsonRegularExpression(mainCategoryRegex, "i")));

            if (subCategory != null)
            {
                var subCategoryRegex = CategoryEnumHelper.GetSubCategoryRegex(mainCategory, subCategory);
                if (!string.IsNullOrEmpty(subCategoryRegex))
                    filters.Add(Builders<Product>.Filter.Regex(x => x.Categories, new BsonRegularExpression(subCategoryRegex, "i")));
            }
        }

        // Ingredient Filtering
        if (restrictions != null)
        {
            foreach (var restriction in restrictions)
            {
                var restrictionRegex = IngredientEnumHelper.GetFoodRestrictionRegex(restriction);
                filters.Add(
                    Builders<Product>.Filter.And(
                        Builders<Product>.Filter.Not(Builders<Product>.Filter.Regex(x => x.IngredientsText, new BsonRegularExpression(restrictionRegex, "i"))),
                        Builders<Product>.Filter.Not(Builders<Product>.Filter.Regex(x => x.ProductName, new BsonRegularExpression(restrictionRegex, "i")))
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
                    Builders<Product>.Filter.And(
                        Builders<Product>.Filter.Not(Builders<Product>.Filter.Regex(x => x.IngredientsText, new BsonRegularExpression(ingredientRegex, "i"))),
                        Builders<Product>.Filter.Not(Builders<Product>.Filter.Regex(x => x.ProductName, new BsonRegularExpression(ingredientRegex, "i")))
                    )
                );
            }
        }

        if (excludedCustomIngredients != null)
        {
            foreach (var customIngredient in excludedCustomIngredients)
            {
                var escapedIngredient = Regex.Escape(customIngredient.Trim());
                var customIngredientRegex = $@"\b{escapedIngredient}s?\b";
                filters.Add(
                    Builders<Product>.Filter.And(
                        Builders<Product>.Filter.Not(Builders<Product>.Filter.Regex(x => x.IngredientsText, new BsonRegularExpression(customIngredientRegex, "i"))),
                        Builders<Product>.Filter.Not(Builders<Product>.Filter.Regex(x => x.ProductName, new BsonRegularExpression(customIngredientRegex, "i")))
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
                filters.Add(Builders<Product>.Filter.Gt(x => x.Id, prevLastIdShown));
            }
        }
        else if (idsShown.Count > 0 && !prevPage)
        {
            filters.Add(Builders<Product>.Filter.Gt(x => x.Id, idsShown.Last()));
        }

        var finalFilter = filters.Count > 0 ? Builders<Product>.Filter.And(filters)
                                            : Builders<Product>.Filter.Empty;

        var foodItems = await _dbService.FindAsync(finalFilter, findOptions).ConfigureAwait(false);
        if (foodItems.Count == 0)
            return [];

        if (!prevPage)
            idsShown.Add(foodItems.Last().Id!);

        await _cache.SetCacheValue($"{_idsShownKey}-{sessionId}", string.Join(',', idsShown), 30).ConfigureAwait(false);

        return foodItems;
    }

    public async Task<List<ProductPreviewsResponse>> GetProductPreviewsByIdsAsync(List<string> ids)
    {
        if (ids.Count == 0)
            return [];

        var findFilter = Builders<Product>.Filter.In(x => x.Id, ids);
        var findOptions = new FindOptions<Product, ProductPreviewsResponse>
        {
            Limit = _itemsPerPage,
            Projection = Builders<Product>.Projection.Expression(x =>
                new ProductPreviewsResponse
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

    public async Task<UpdateResponse> UpdateCompleteProductAsync(Product product)
    {
        var filter = Builders<Product>.Filter.Eq(x => x.Id, product.Id);
        
        return await _dbService.ReplaceOneAsync(product).ConfigureAwait(false);
    }

    public string? BuildImageUrl(List<Image> images, string code, ImageType type)
    {
        var rev = images.FirstOrDefault(x => x.ImageType == type)?.Rev;
        if (rev == null)
            return null;

        var barcode = code.PadLeft(13, '0');
        var splitMatch = Regex.Match(barcode, _barcodeSplitPattern);

        var imageName = type switch
        {
            ImageType.Front => _frontImageName,
            ImageType.Nutrition => _nutritionImageName,
            ImageType.Ingredients => _ingredientsImageName,
            ImageType.Packaging => _packagingImageName,
            _ => string.Empty
        };

        var folderName = $"{splitMatch.Groups[1].Value}/{splitMatch.Groups[2].Value}/{splitMatch.Groups[3].Value}/{splitMatch.Groups[4].Value}";
        var fileName = $"{imageName}.{rev}.400.jpg";

        return $"{_imageBaseUrl}/{folderName}/{fileName}";
    }
}
