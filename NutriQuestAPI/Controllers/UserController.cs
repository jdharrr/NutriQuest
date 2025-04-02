using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriQuestServices.ProductServices;
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
    public async Task<IActionResult> AddProductToFavoritesAsync([FromQuery] FavoritesAddRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.AddProductToFavoritesAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ProductExistsException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpDelete("deleteFromFavorites")]
    public async Task<IActionResult> DeleteProductFromFavoritesAsync([FromQuery] FavoritesDeleteRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.DeleteProductFromFavoritesAsync(request).ConfigureAwait(false));
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

    [HttpDelete("clearFavorites")]
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
    public async Task<IActionResult> AddProductToCartAsync([FromQuery] AddToCartRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.AddProductToCartAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ProductExistsException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpDelete("deleteFromCart")]
    public async Task<IActionResult> DeleteProductFromFavoritesAsync([FromQuery] DeleteFromCartRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.DeleteProductFromCartAsync(request).ConfigureAwait(false));
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

    [HttpDelete("clearCart")]
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

    [HttpPost("getNutrients")]
    public async Task<IActionResult> GetTrackedNutrientsAsync([FromBody] NutrientsRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.GetTrackedNutrientsAsync(request).ConfigureAwait(false));
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

    [HttpPost("addNutrients")]
    public async Task<IActionResult> AddNutrientsEntryAsync([FromBody] AddNutrientsRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _userService.AddNutrientsEntryAsync(request).ConfigureAwait(false));
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