using DatabaseServices.Models;
using Microsoft.AspNetCore.Mvc;
using NutriQuestServices;

namespace NutriQuestAPI.Controllers;

[ApiController]
[Route("nutriQuestApi/foodItems")]
public class FoodItemsController : ControllerBase
{
    FoodService _foodService;

    public FoodItemsController(FoodService foodService)
    {
        _foodService = foodService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFoodItemById(string id)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(id, out var _))
            return BadRequest("Invalid Parameter");

        FoodItem? item = await _foodService.GetFoodItemById(id).ConfigureAwait(false);
        if (item == null)
            return NotFound();

        return Ok(item);
    }
}