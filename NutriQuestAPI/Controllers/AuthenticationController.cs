using AuthenticationServices;
using AuthenticationServices.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace NutriQuestAPI.Controllers;

[ApiController]
[Route("nutriQuestApi/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationService _authService;

    public AuthenticationController(AuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost("createNewUser")]
    public async Task<IActionResult> CreateNewUser([FromBody] NewUserRequest request)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(request.Email, emailPattern, RegexOptions.IgnoreCase))
            return BadRequest("Incorrect email format.");

        var userCreated = await _authService.CreateNewUserAsync(request).ConfigureAwait(false);
        if (!userCreated)
            return BadRequest("Email already exists.");

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request).ConfigureAwait(false);
        if (response == null || string.IsNullOrEmpty(response.Token))
            return BadRequest("Email or password are inccorect.");

        return Ok(response);
    }

    [Authorize]
    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        var response = await _authService.ChangePasswordAsync(request);
        if (response == null)
            return NotFound("User not found");

        if (response.ChangeSuccess == null)
            return Unauthorized("Incorrect current password");

        return Ok(response);
    }
}
