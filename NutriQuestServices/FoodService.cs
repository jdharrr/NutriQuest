using DatabaseServices;
using DatabaseServices.Models;
using MongoDB.Driver;

namespace NutriQuestServices;

public class FoodService
{
    private readonly DatabaseService<FoodItem> _dbService;

    public FoodService(DatabaseService<FoodItem> databaseService)
    {
        _dbService = databaseService;
    }

    public async Task<FoodItem?> GetFoodItemByIdAsync(string id)
    {
        var filter = Builders<FoodItem>.Filter.Eq(x => x.Id, id);

        return await _dbService.FindOneAsync(filter).ConfigureAwait(false);
    }
}