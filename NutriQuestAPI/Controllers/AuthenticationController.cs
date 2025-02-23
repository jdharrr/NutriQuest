using AuthenticationServices;
using AuthenticationServices.Requests;
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
        var userId = await _authService.LoginAsync(request).ConfigureAwait(false);
        if (string.IsNullOrEmpty(userId))
            return BadRequest("Email or password are inccorect.");

        return Ok(userId);
    }
}
