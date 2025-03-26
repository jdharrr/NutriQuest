using Microsoft.AspNetCore.Mvc;
using NutriQuestServices.IngredientService;
using NutriQuestServices.IngredientService.Requests;

namespace NutriQuestAPI.Controllers;

[ApiController]
[Route("nutriQuestApi/ingredients")]
public class IngredientController : ControllerBase
{
	private readonly IngredientService _ingredientService;

	public IngredientController(IngredientService ingredientService)
	{
		_ingredientService = ingredientService;
	}

	[HttpGet("validateIngredient")]
	public async Task<IActionResult> ValidateIngredientAsync([FromQuery] CustomIngredientRequest request)
	{
		try
		{
			return Ok(await _ingredientService.ValidateCustomIngredientAsync(request).ConfigureAwait(false));
		}
		catch (Exception)
		{
			return Problem("An error occurred while processing the request.");
		}
	}
}
