using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriQuestServices.UserServices;
using NutriQuestServices.UserServices.Requests;

namespace NutriQuestAPI.Controllers;

[Authorize]
[ApiController]
[Route("nutriQuestApi/user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("getUserAccount")]
    public async Task<IActionResult> GetUserAccountAsync([FromQuery] UserAccountRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        var response = await _userService.GetUserAccountAsync(request).ConfigureAwait(false);
        if (response == null)
            return NotFound("User not found");

        return Ok(response);
    }

    [HttpGet("addToFavorites")]
    public async Task<IActionResult> AddItemToFavoritesAsync([FromQuery] FavoritesAddRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        var response = await _userService.AddItemToFavoritesAsync(request).ConfigureAwait(false);
        if (response == null)
            return NotFound("User not found");

        return Ok(response);
    }

    [HttpGet("deleteFromFavorites")]
    public async Task<IActionResult> DeleteItemFromFavoritesAsync([FromQuery] FavoritesDeleteRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        var response = await _userService.DeleteItemFromFavoritesAsync(request).ConfigureAwait(false);
        if (response == null)
            return NotFound("User not found");

        return Ok(response);
    }

    [HttpGet("clearFavorites")]
    public async Task<IActionResult> ClearFavoritesAsync([FromQuery] FavoritesClearRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        var response = await _userService.ClearFavoritesAsync(request).ConfigureAwait(false);
        if (response == null)
            return NotFound("User not found");

        return Ok(response);
    }

    [HttpGet("getFavorites")]
    public async Task<IActionResult> GetFavoritesAsync([FromQuery] GetFavoritesRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        var response = await _userService.GetFavoritesAsync(request).ConfigureAwait(false);
        if (response == null)
            return NotFound("User not found");

        return Ok(response);
    }

    [HttpGet("addToCart")]
    public async Task<IActionResult> AddItemToCartAsync([FromQuery] AddToCartRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        var response = await _userService.AddItemToCartAsync(request).ConfigureAwait(false);
        if (response == null)
            return NotFound("User not found");

        return Ok(response);
    }

    [HttpGet("deleteFromCart")]
    public async Task<IActionResult> DeleteItemFromFavoritesAsync([FromQuery] DeleteFromCartRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        var response = await _userService.DeleteItemFromCartAsync(request).ConfigureAwait(false);
        if (response == null)
            return NotFound("User not found");

        return Ok(response);
    }

    [HttpGet("clearCart")]
    public async Task<IActionResult> ClearCartAsync([FromQuery] ClearCartRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        var response = await _userService.ClearCartAsync(request).ConfigureAwait(false);
        if (response == null)
            return NotFound("User not found");

        return Ok(response);
    }

    [HttpGet("getCart")]
    public async Task<IActionResult> GetCartAsync([FromQuery] GetCartRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        var response = await _userService.GetCartAsync(request).ConfigureAwait(false);
        if (response == null)
            return NotFound("User not found");

        return Ok(response);
    }
}