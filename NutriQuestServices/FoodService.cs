using CacheServices;
using DatabaseServices;
using DatabaseServices.Models;
using MongoDB.Driver;
using NutriQuestServices.FoodResponses;
using StackExchange.Redis;

namespace NutriQuestServices;

public class FoodService
{
    private readonly DatabaseService<FoodItem> _dbService;
    
    private readonly string _idsShownKey = "idsShown";
    
    private readonly CacheService _cache;

    private readonly int _itemsPerPage = 9;

    public FoodService(DatabaseService<FoodItem> databaseService, CacheService cache)
    {
        _dbService = databaseService;
        _cache = cache;
    }

    public async Task<FoodItem?> GetFoodItemByIdAsync(string id)
    {
        var findOptions = new FindOptions<FoodItem>
        {
            Projection = Builders<FoodItem>.Projection.Exclude(x => x.Images)
                .Exclude(x => x.MaxImgId)
                .Exclude(x => x.Rev)
        };
        var filter = Builders<FoodItem>.Filter.Eq(x => x.Id, id);

        return await _dbService.FindOneAsync(filter, findOptions).ConfigureAwait(false);
    }

    public async Task<string> GetFoodItemFrontImgUrlAsync(string id)
    {
        return string.Empty;
    }

    public async Task<List<string>> GetAllFoodItemImgUrlsAsync(string id)
    {
        return [];
    }
    
    // TODO: Make the cache values user based. Currently all connections to the api are using same cache keys.
    public async Task<List<FoodItemPreviewResponse>> GetFoodItemPreviewsAsync(bool prevPage)
    {
        var idsShownValue = await _cache.GetCacheValue(_idsShownKey);
        List<string> idsShown = [];
        if (!string.IsNullOrEmpty(idsShownValue))
            idsShown = idsShownValue.Split(',').ToList();

        if (prevPage == true && idsShown.Count == 0)
            return [];
        
        var findOptions = new FindOptions<FoodItem, FoodItemPreviewResponse>
        {
            Limit = _itemsPerPage,
            Sort = Builders<FoodItem>.Sort.Ascending(x => x.Id),
            Projection = Builders<FoodItem>.Projection.Expression(x =>
                new FoodItemPreviewResponse
                {
                    Id = x.Id,
                    Name = x.ProductName,
                    Price = x.Price,
                    StoresInStock = x.StoresInStock,
                    Brands = x.Brands,
                    Rating = x.Rating
                })
        };

        FilterDefinition<FoodItem> filter;
        if (idsShown.Count == 0)
        {
            filter = Builders<FoodItem>.Filter.Empty;
        }
        else if (prevPage == true)
        {
            if (idsShown.Count < 3)
            {
                filter = Builders<FoodItem>.Filter.Empty;
            }
            else
            {
                var prevLastIdShown = idsShown[^3];
                idsShown.RemoveAt(idsShown.Count - 1);
                filter = Builders<FoodItem>.Filter.Gt(x => x.Id, prevLastIdShown);
            }
        }
        else
        {
            filter = Builders<FoodItem>.Filter.Gt(x => x.Id, idsShown[^1]);
        }

        var foodItems = await _dbService.FindAsync(filter, findOptions);
        if (foodItems.Count == 0)
            return [];
        
        if (prevPage != true)
        {
            idsShown.Add(foodItems.Last().Id!);
        }

        await _cache.SetCacheValue(_idsShownKey, string.Join(',', idsShown));

        return foodItems;
    }
}