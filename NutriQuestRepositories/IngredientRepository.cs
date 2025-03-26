using DatabaseServices;
using DatabaseServices.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace NutriQuestRepositories;

public class IngredientRepository
{
	private readonly DatabaseService<Ingredient> _dbService;

	public IngredientRepository(DatabaseService<Ingredient> dbService)
	{
		_dbService = dbService;
	}

    public async Task<Ingredient?> ValidateCustomIngredientAsync(string ingredient)
    {
        var options = new FindOptions<Ingredient>()
        {
            Projection = Builders<Ingredient>.Projection.Exclude(x => x.AllIngredients)
        };
        var filter = Builders<Ingredient>.Filter.Regex(x => x.AllIngredients, new BsonRegularExpression($"\\b{ingredient}s?\\b", "i"));

        return await _dbService.FindOneAsync(filter).ConfigureAwait(false);
    }
}
