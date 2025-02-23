using DatabaseServices.Models;
using Microsoft.AspNetCore.Mvc;
using NutriQuestServices;
using NutriQuestServices.FoodRequests;

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

    [HttpGet("itemById")]
    public async Task<IActionResult> GetFoodItemByIdAsync([FromQuery] FoodItemByIdRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.ItemId, out var _))
            return BadRequest("Invalid Parameter");

        FoodItem? item = await _foodService.GetFoodItemByIdAsync(request).ConfigureAwait(false);
        if (item == null)
            return NotFound();

        return Ok(item);
    }

    [HttpGet("itemPreviews")]
    public async Task<IActionResult> GetFoodItemPreviewsAsync([FromQuery] FoodItemPreviewsRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        return Ok(await _foodService.GetFoodItemPreviewsAsync(request).ConfigureAwait(false));
    }

    [HttpGet("itemFrontImage")]
    public async Task<IActionResult> GetFoodItemFrontImageAsync([FromQuery] FoodImageRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.ItemId, out var _))
            return BadRequest("Invalid Parameter");

        var url = await _foodService.GetFoodItemFrontImgUrlAsync(request).ConfigureAwait(false);
        if (string.IsNullOrEmpty(url))
            return NotFound();

        return Ok(url);
    }

    [HttpGet("itemAllImages")]
    public async Task<IActionResult> GetFoodItemAllImagesAsync([FromQuery] FoodImageRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.ItemId, out var _))
            return BadRequest("Invalid Parameter");

        var images = await _foodService.GetFoodItemAllImgUrlsAsync(request).ConfigureAwait(false);
        if (images == null)
            return NotFound();

        return Ok(images);
    }
}