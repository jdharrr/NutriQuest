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

    public async Task<List<ProductPreviewsResponse>> GetProductPreviewsPagingAsync(string sessionId, bool prevPage, bool restartPaging, string mainCategory, string subCategory, List<string> restrictions, List<string> excludedIngredients, List<string> excludedCustomIngredients, string sort)
    {
        var findOptions = new FindOptions<Product, ProductPreviewsResponse>
        {
            Limit = _itemsPerPage,
            Projection = Builders<Product>.Projection.Expression(x =>
                new ProductPreviewsResponse
                {
                    Id = x.Id,
                    ProductName = x.ProductName ?? "Unknown Product",
                    Price = x.Price ?? 0.0,
                    StoresInStock = x.StoresInStock ?? new List<string>(),
                    Brands = x.Brands ?? "Unkown Brand",
                    Rating = x.Rating
                }
            )
        };
        
        // Sorting
        var sortProperty = "_id";
        var sortPropertyType = typeof(string);
        if (!string.IsNullOrEmpty(sort))
        {
            var propertyName = SortOptionsHelper.GetProductPropertyForSort(sort);
            if (!string.IsNullOrEmpty(propertyName))
            {
                sortProperty = propertyName;

                var productType = typeof(Product);
                var sortPropertyInfo = productType.GetProperty(char.ToUpper(propertyName[0]) + propertyName[1..]);
                if (sortPropertyInfo != null)
                    sortPropertyType = sortPropertyInfo.PropertyType;
            }
        }

        if (!string.IsNullOrEmpty(sort))
        {
            var sortDefinition = Builders<Product>.Sort.Combine(sort.Contains("Descending") ? Builders<Product>.Sort.Descending(sortProperty)
                                                                                            : Builders<Product>.Sort.Ascending(sortProperty),
                                                                                              Builders<Product>.Sort.Ascending(x => x.Id));
            findOptions.Sort = sortDefinition;
        }

        // Collection of all needed filters for find query
        List<FilterDefinition<Product>> filters = [];

        // Category Filtering
        if (!string.IsNullOrEmpty(mainCategory))
        {
            var mainCategoryRegex = CategoryEnumHelper.GetMainFoodCategoryRegex(mainCategory);
            if (!string.IsNullOrEmpty(mainCategoryRegex))
                filters.Add(Builders<Product>.Filter.Regex(x => x.Categories, new BsonRegularExpression(mainCategoryRegex, "i")));

            if (!string.IsNullOrEmpty(subCategory))
            {
                var subCategoryRegex = CategoryEnumHelper.GetSubCategoryRegex(mainCategory, subCategory);
                if (!string.IsNullOrEmpty(subCategoryRegex))
                    filters.Add(Builders<Product>.Filter.Regex(x => x.Categories, new BsonRegularExpression(subCategoryRegex, "i")));
            }
        }

        // Ingredient Filtering
        if (restrictions.Count > 0)
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

        if (excludedIngredients.Count > 0)
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

        if (excludedCustomIngredients.Count > 0)
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
        List<CacheRecord> recordsShown = [];
        if (restartPaging)
        {
            await _cache.DeleteCacheValue(sessionId);
        }
        else
        {
            var idsShownValue = await _cache.GetCacheValue($"{_idsShownKey}-{sessionId}").ConfigureAwait(false);
            if (!string.IsNullOrEmpty(idsShownValue))
                recordsShown = JsonSerializer.Deserialize<List<CacheRecord>>(idsShownValue) ?? [];
        }

        if (prevPage && recordsShown.Count > 1)
        {
            if (recordsShown.Count > 2)
            {
                var prevLastRecord = recordsShown[^3];
                filters.Add(BuildPreviewsPaginationFilter(prevLastRecord, sort, sortProperty, sortPropertyType));
            }

            recordsShown.RemoveAt(recordsShown.Count - 1);
        }
        else if (recordsShown.Count > 0 && !prevPage)
        {
            filters.Add(BuildPreviewsPaginationFilter(recordsShown.Last(), sort, sortProperty, sortPropertyType));
        }

        var finalFilter = filters.Count > 0 ? Builders<Product>.Filter.And(filters)
                                            : Builders<Product>.Filter.Empty;

        var foodItems = await _dbService.FindAsync(finalFilter, findOptions).ConfigureAwait(false);
        if (foodItems.Count == 0)
            return [];

        if (!prevPage)
        {
            if (!string.IsNullOrEmpty(sort))
            {
                var sortPropertyValue = foodItems.Last().GetType().GetProperty(char.ToUpper(sortProperty[0]) + sortProperty[1..])?.GetValue(foodItems.Last())?.ToString();
                if (sortPropertyValue != null)
                    recordsShown.Add(new CacheRecord { Id = foodItems.Last().Id!, Sort = sortPropertyValue });
            }
            else
            {
                recordsShown.Add(new CacheRecord { Id = foodItems.Last().Id!, Sort = foodItems.Last().Id!} );
            }
        }

        var pageInfoJson = JsonSerializer.Serialize(recordsShown);
        await _cache.SetCacheValue($"{_idsShownKey}-{sessionId}", pageInfoJson, 20).ConfigureAwait(false);

        return foodItems;
    }

    private static FilterDefinition<Product> BuildPreviewsPaginationFilter(CacheRecord prevRecord, string? sort, string sortProperty, Type sortType)
    {
        var idFilter = Builders<Product>.Filter.Gt(x => x.Id, prevRecord.Id);
        if (sort == null)
            return idFilter;

        FilterDefinition<Product> sortFilter;
        if (sortType == typeof(string))
        {
            sortFilter = sort.Contains("Descending")
                ? Builders<Product>.Filter.Lt(sortProperty, prevRecord.Sort)
                : Builders<Product>.Filter.Gt(sortProperty, prevRecord.Sort);
        }
        else
        {
            sortFilter = sort.Contains("Descending")
                ? Builders<Product>.Filter.Lt(sortProperty, Convert.ToInt32(prevRecord.Sort))
                : Builders<Product>.Filter.Gt(sortProperty, Convert.ToInt32(prevRecord.Sort));
        }

        return Builders<Product>.Filter.Or(
            sortFilter,
            Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(sortProperty, prevRecord.Sort),
                idFilter
            )
        );
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
                    ProductName = x.ProductName ?? "Unknown Product",
                    Price = x.Price ?? 0.0,
                    StoresInStock = x.StoresInStock ?? new List<string>(),
                    Brands = x.Brands ?? "Unknown Brand",
                    Rating = x.Rating
                }
           )
        };

        return await _dbService.FindAsync(findFilter, findOptions).ConfigureAwait(false);
    }

    public async Task<CartResponse> GetCartPreviewsAsync(Cart cart)
    {
        var response = new CartResponse();
        
        if (cart.Products.Count == 0)
            return response;

        var findFilter = Builders<Product>.Filter.In(x => x.Id, cart.Products.Select(x => x.ProductId));
        var findOptions = new FindOptions<Product, CartProductPreview>
        {
            Limit = _itemsPerPage,
            Projection = Builders<Product>.Projection.Expression(x =>
                new CartProductPreview
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    Price = x.Price ?? 0.0,
                    StoresInStock = x.StoresInStock,
                }
           )
        };

        var previews = await _dbService.FindAsync(findFilter, findOptions).ConfigureAwait(false);
        foreach (var item in previews)
        {
            var productRef = cart.Products.Find(x => x.ProductId == item.Id);
            item.NumberOfProduct = productRef!.NumberOfProduct;
        }

        response.Products = previews;
        response.TotalPrice = cart.TotalPrice;
        
        return response;
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
