using DatabaseServices;
using DatabaseServices.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NutriQuestServices.IngredientService.Requests;
using NutriQuestServices.IngredientService.Responses;
using System.Text.RegularExpressions;

namespace NutriQuestServices.IngredientService;

public class IngredientService
{
	private readonly DatabaseService<Ingredient> _dbService;

	public IngredientService(DatabaseService<Ingredient> dbService)
	{
		_dbService = dbService;
	}

    public async Task<CustomIngredientResponse> ValidateCustomIngredientAsync(CustomIngredientRequest request)
    {
        var response = new CustomIngredientResponse();
        var escapedIngredient = Regex.Escape(request.CustomIngredient.Trim());

        var options = new FindOptions<Ingredient>()
        {
            Projection = Builders<Ingredient>.Projection.Exclude(x => x.AllIngredients)
        };
        var filter = Builders<Ingredient>.Filter.Regex(x => x.AllIngredients, new BsonRegularExpression($"\\b{escapedIngredient}s?\\b", "i"));
        
        var ingredient = await _dbService.FindOneAsync(filter).ConfigureAwait(false);
        response.ValidIngredient = ingredient != null;

        return response;
    }
}
