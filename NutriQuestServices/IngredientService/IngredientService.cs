using NutriQuestRepositories;
using NutriQuestServices.IngredientService.Requests;
using NutriQuestServices.IngredientService.Responses;
using System.Text.RegularExpressions;

namespace NutriQuestServices.IngredientService;

public class IngredientService
{
	private readonly IngredientRepository _ingredientRepo;

	public IngredientService(IngredientRepository ingredientRepo)
	{
        _ingredientRepo = ingredientRepo;
	}

    public async Task<CustomIngredientResponse> ValidateCustomIngredientAsync(CustomIngredientRequest request)
    {
        var response = new CustomIngredientResponse();
        
        var escapedIngredient = Regex.Escape(request.CustomIngredient.Trim());
        response.ValidIngredient = await _ingredientRepo.ValidateCustomIngredientAsync(escapedIngredient).ConfigureAwait(false) != null;

        return response;
    }
}
