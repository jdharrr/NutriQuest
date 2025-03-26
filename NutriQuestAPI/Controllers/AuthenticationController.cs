using AuthenticationServices;
using AuthenticationServices.Requests;
using EmailServices.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriQuestServices.UserServices;
using System.Text.RegularExpressions;

namespace NutriQuestAPI.Controllers;

[ApiController]
[Route("nutriQuestApi/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationService _authService;

    private readonly string _emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    private readonly string _genericProblemResponse = "An error occurred while processing the request.";

    public AuthenticationController(AuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost("createNewUser")]
    public async Task<IActionResult> CreateNewUser([FromBody] NewUserRequest request)
    {
        if (!Regex.IsMatch(request.Email, _emailPattern, RegexOptions.IgnoreCase))
            return BadRequest("Incorrect email format.");

        bool userCreated;
        try
        {
            userCreated = await _authService.CreateNewUserAsync(request).ConfigureAwait(false);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
        
        if (!userCreated)
            return BadRequest("Email already exists.");

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        try
        {
            return Ok(await _authService.LoginAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException)
        {
            return Unauthorized("Email or password are incorrect.");
        }
        catch (InvalidPasswordException) 
        {
            return Unauthorized("Email or password are inccorect.");
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [Authorize]
    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _authService.ChangePasswordAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidPasswordException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("forgotPassword")]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequest request)
    {
        if (!Regex.IsMatch(request.Email, _emailPattern, RegexOptions.IgnoreCase))
            return BadRequest("Incorrect email format.");

        try
        {
            return Ok(await _authService.ForgotPasswordAsync(request).ConfigureAwait(false));
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

    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
    {
        try
        {
            return Ok(await _authService.ResetPasswordAsync(request).ConfigureAwait(false));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidTokenException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }
}
