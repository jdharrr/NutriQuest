using Microsoft.AspNetCore.Mvc;
using NutriQuestServices.FoodServices;
using NutriQuestServices.FoodServices.Requests;

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

        var response = await _foodService.GetFoodItemByIdAsync(request).ConfigureAwait(false);
        if (response.FoodItem == null)
            return NotFound("Item not found");

        return Ok(response);
    }

    [HttpGet("itemPreviews")]
    public async Task<IActionResult> GetFoodItemPreviewsAsync([FromQuery] FoodItemPreviewsRequest request)
    {
        return Ok(await _foodService.GetFoodItemPreviewsAsync(request).ConfigureAwait(false));
    }

    [HttpGet("itemFrontImage")]
    public async Task<IActionResult> GetFoodItemFrontImageAsync([FromQuery] FoodImageRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.ItemId, out var _))
            return BadRequest("Invalid Parameter");

        var response = await _foodService.GetFoodItemFrontImgUrlAsync(request).ConfigureAwait(false);
        if (string.IsNullOrEmpty(response.Url))
            return NotFound();

        return Ok(response);
    }

    [HttpGet("itemAllImages")]
    public async Task<IActionResult> GetFoodItemAllImagesAsync([FromQuery] FoodImageRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.ItemId, out var _))
            return BadRequest("Invalid Parameter");

        var images = await _foodService.GetFoodItemAllImgUrlsAsync(request).ConfigureAwait(false);
        if (images == null || images.Images.Count == 0)
            return NotFound();

        return Ok(images);
    }
}