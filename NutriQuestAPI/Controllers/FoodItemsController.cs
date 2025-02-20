using DatabaseServices.Models;
using Microsoft.AspNetCore.Mvc;
using NutriQuestServices;

namespace NutriQuestAPI.Controllers;

[ApiController]
[Route("nutriQuestApi/foodItems")]
public class FoodItemsController : ControllerBase
{
    private readonly FoodService _foodService;

    public FoodItemsController(FoodService foodService)
    {
        _foodService = foodService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFoodItemByIdAsync(string id)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(id, out var _))
            return BadRequest("Invalid Parameter");

        FoodItem? item = await _foodService.GetFoodItemByIdAsync(id).ConfigureAwait(false);
        if (item == null)
            return NotFound();

        return Ok(item);
    }
}