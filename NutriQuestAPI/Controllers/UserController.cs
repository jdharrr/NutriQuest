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

    private readonly string _genericProblemResponse = "An error occurred while processing the request.";

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("getAccount")]
    public async Task<IActionResult> GetUserAccountAsync([FromQuery] UserAccountRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.GetUserAccountAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("addToFavorites")]
    public async Task<IActionResult> AddItemToFavoritesAsync([FromQuery] FavoritesAddRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.AddItemToFavoritesAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("deleteFromFavorites")]
    public async Task<IActionResult> DeleteItemFromFavoritesAsync([FromQuery] FavoritesDeleteRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.DeleteItemFromFavoritesAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("clearFavorites")]
    public async Task<IActionResult> ClearFavoritesAsync([FromQuery] FavoritesClearRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.ClearFavoritesAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("getFavorites")]
    public async Task<IActionResult> GetFavoritesAsync([FromQuery] GetFavoritesRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");
        try
        {
            return Ok(await _userService.GetFavoritesAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("addToCart")]
    public async Task<IActionResult> AddItemToCartAsync([FromQuery] AddToCartRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.AddItemToCartAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("deleteFromCart")]
    public async Task<IActionResult> DeleteItemFromFavoritesAsync([FromQuery] DeleteFromCartRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.DeleteItemFromCartAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("clearCart")]
    public async Task<IActionResult> ClearCartAsync([FromQuery] ClearCartRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.ClearCartAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("getCart")]
    public async Task<IActionResult> GetCartAsync([FromQuery] GetCartRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.GetCartAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("getRatings")]
    public async Task<IActionResult> GetUserRatingsAsync([FromQuery] UserRatingsRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.GetUserRatingsAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }
}