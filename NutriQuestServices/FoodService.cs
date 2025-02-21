using DatabaseServices;
using DatabaseServices.Models;
using MongoDB.Driver;
using NutriQuestServices.FoodResponses;

namespace NutriQuestServices;

public class FoodService
{
    private readonly DatabaseService<FoodItem> _dbService;

    private string? _lastIdShown = null;

    private readonly int _itemsPerPage = 8;

    public FoodService(DatabaseService<FoodItem> databaseService)
    {
        _dbService = databaseService;
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

    public async Task<List<FoodItemPreviewResponse>> GetFoodItemPreviewsAsync()
    {
        var findOptions = new FindOptions<FoodItem, FoodItemPreviewResponse>
        {
            Limit = _itemsPerPage,
            Projection = Builders<FoodItem>.Projection.Expression(x =>
            new FoodItemPreviewResponse {
                Name = x.ProductName,
                Price = x.Price,
                StoresInStock = x.StoresInStock,
                Brands = x.Brands,
                Rating = x.Rating
            })
        };

        var filter = _lastIdShown == null ? Builders<FoodItem>.Filter.Empty
                                          : Builders<FoodItem>.Filter.Gt(x => x.Id, _lastIdShown);

        var foodItems = await _dbService.FindAsync(filter, findOptions);

        return foodItems;
    }
}