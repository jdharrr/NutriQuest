using System.Linq.Expressions;
using System.Text.Json;
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
        var sortProperty = "_id";
        var sortPropertyType = typeof(string);
        if (sort != null)
        {
            var property = SortOptionsHelper.GetProductPropertyForSort(sort);
            if (!string.IsNullOrEmpty(property))
                sortProperty = property;
            
            var productType = typeof(Product);
            var sortproperty = productType.GetProperty(char.ToUpper(sort[0]) + sort[1..]);
            if (sortproperty != null)
                sortPropertyType = sortproperty.PropertyType;
        }
        
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
        
        // Sorting
        if (sort != null)
        {
            SortDefinition<Product> sortDefinition;
            if (sort.Contains("Descending"))
            {
                sortDefinition = Builders<Product>.Sort.Descending(sortProperty)
                                                       .Ascending(x => x.Id);
            }
            else
            {
                sortDefinition = Builders<Product>.Sort.Ascending(sortProperty)
                                                       .Descending(x => x.Id);
            }

            findOptions.Sort = sortDefinition;
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
        List<(string Sort, string Id)> recordsShown = [];
        if (restartPaging)
        {
            await _cache.DeleteCacheValue(sessionId);
        }
        else
        {
            var idsShownValue = await _cache.GetCacheValue($"{_idsShownKey}-{sessionId}").ConfigureAwait(false);
            if (!string.IsNullOrEmpty(idsShownValue))
                recordsShown = JsonSerializer.Deserialize<List<(string, string)>>(idsShownValue);
        }
        
        if (prevPage && recordsShown.Count > 1)
        {
            if (recordsShown.Count < 3)
            {
                recordsShown.RemoveAt(1);
            }
            else
            {
                var prevLastRecord = recordsShown[^3];
                recordsShown.RemoveAt(recordsShown.Count - 1);

                if (sort != null)
                {
                    if (sortPropertyType == typeof(string))
                    {
                        filters.Add(sort.Contains("Descending")
                            ? Builders<Product>.Filter.Lt(sortProperty, prevLastRecord.Sort)
                            : Builders<Product>.Filter.Gt(sortProperty, prevLastRecord.Sort));
                    }
                    else
                    {
                        filters.Add(sort.Contains("Descending")
                            ? Builders<Product>.Filter.Lt(sortProperty, Convert.ToInt32(prevLastRecord.Sort))
                            : Builders<Product>.Filter.Gt(sortProperty, Convert.ToInt32(prevLastRecord.Sort)));
                    }
                }

                filters.Add(Builders<Product>.Filter.Gt(x => x.Id, prevLastRecord.Id));
            }
        }
        else if (recordsShown.Count > 0 && !prevPage)
        {
            if (sortPropertyType == typeof(string))
            {
                filters.Add(sort.Contains("Descending")
                    ? Builders<Product>.Filter.Lt(sortProperty, recordsShown.Last().Sort)
                    : Builders<Product>.Filter.Gt(sortProperty, recordsShown.Last().Sort));
            }
            else
            {
                filters.Add(sort.Contains("Descending")
                    ? Builders<Product>.Filter.Lt(sortProperty, Convert.ToInt32(recordsShown.Last().Sort))
                    : Builders<Product>.Filter.Gt(sortProperty, Convert.ToInt32(recordsShown.Last().Sort)));
            }
            
            filters.Add(Builders<Product>.Filter.Gt(x => x.Id, recordsShown.Last().Id));
        }

        var finalFilter = filters.Count > 0 ? Builders<Product>.Filter.And(filters)
                                            : Builders<Product>.Filter.Empty;

        var foodItems = await _dbService.FindAsync(finalFilter, findOptions).ConfigureAwait(false);
        if (foodItems.Count == 0)
            return [];

        if (!prevPage)
        {
            var productType = typeof(Product);
            var property = productType.GetProperty(char.ToUpper(sort[0]) + sort[1..]);
            recordsShown.Add((Sort: property.GetValue(foodItems.Last()), Id: foodItems.Last().Id!));
        }

        var pageInfoJson = JsonSerializer.Serialize(recordsShown);
        await _cache.SetCacheValue($"{_idsShownKey}-{sessionId}", pageInfoJson, 30).ConfigureAwait(false);

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
