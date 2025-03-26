﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using AuthenticationServices.Requests;
using AuthenticationServices.Responses;
using DatabaseServices.Models;
using EmailServices;
using EmailServices.Requests;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NutriQuestRepositories;
using NutriQuestServices.UserServices;

namespace AuthenticationServices;

public class AuthenticationService
{
    private readonly UserRepository _userRepo;

    private readonly TokenService _tokenService;

    private readonly EmailService _emailService;

    private readonly JwtSettings _jwtSettings;
    
    public AuthenticationService(UserRepository userRepo, TokenService tokenService, EmailService emailService, IOptions<JwtSettings> jwtSettings)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
        _emailService = emailService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<bool> CreateNewUserAsync(NewUserRequest request)
    {
        var existingUser = await _userRepo.GetUserByEmailAsync(request.Email);
        if (existingUser != null)
            return false;

        var user = new User
        {
            Email = request.Email,
            Name = request.Name
        };
        
        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);

        user.Password = Hash(request.Password, salt);
        user.Salt = Convert.ToBase64String(salt);

        await _userRepo.InsertUserAsync(user).ConfigureAwait(false);

        return true;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var response = new LoginResponse();

        var user = await _userRepo.GetUserByEmailAsync(request.Email).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        if (!IsValidHash(request.Password, user.Password!, user.Salt!))
            throw new InvalidPasswordException();    
        
        response.Token = _tokenService.GenerateAccessToken(user.Id);

        return response;
    }

    public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var response = new ChangePasswordResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        if (!IsValidHash(request.CurrentPassword, user.Password!, user.Salt!))
            throw new InvalidPasswordException("Incorrect current password.");

        user.Password = Hash(request.NewPassword, Convert.FromBase64String(user.Salt!));

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.ChangeSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }   

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var response = new ForgotPasswordResponse();

        var user = await _userRepo.GetUserByEmailAsync(request.Email).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        var resetToken = _tokenService.GeneratePasswordResetToken(user.Id, request.Email);

        user.PasswordResetToken = Hash(resetToken, Convert.FromBase64String(user.Salt!));
        user.PasswordResetExpiration = DateTime.UtcNow.AddMinutes(int.Parse(_jwtSettings.AccessExpiryMinutes));

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.SendSuccess = updateResponse.ModifiedCount == 1;

        if (response.SendSuccess == true)
            await _emailService.SendPasswordResetEmail(request.Email, resetToken).ConfigureAwait(false);
        
        return response;
    }

    public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var response = new ResetPasswordResponse();

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(request.ResetToken);
        var emailClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;

        var user = await _userRepo.GetUserByEmailAsync(emailClaim).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        if (user.PasswordResetExpiration == null || user.PasswordResetExpiration < DateTime.UtcNow)
            throw new InvalidTokenException();

        if (Hash(request.ResetToken, Convert.FromBase64String(user.Salt!)) != user.PasswordResetToken)
            throw new InvalidTokenException();

        user.Password = Hash(request.NewPassword, Convert.FromBase64String(user.Salt!));
        user.PasswordResetExpiration = null;
        user.PasswordResetToken = null;

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.ResetSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    private static string Hash(string password, byte[] salt)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var combinedBytes = new byte[salt.Length + passwordBytes.Length];
        Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
        Buffer.BlockCopy(passwordBytes, 0, combinedBytes, salt.Length, passwordBytes.Length);
        
        return Convert.ToBase64String(SHA256.HashData(combinedBytes));
    }

    private static bool IsValidHash(string givenHash, string storedHash, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        byte[] givenPasswordBytes = Encoding.UTF8.GetBytes(givenHash);
        byte[] combineBytes = new byte[saltBytes.Length + givenPasswordBytes.Length];

        Buffer.BlockCopy(saltBytes, 0, combineBytes, 0, saltBytes.Length);
        Buffer.BlockCopy(givenPasswordBytes, 0, combineBytes, saltBytes.Length, givenPasswordBytes.Length);

        byte[] hash = SHA256.HashData(combineBytes);
        
        return Convert.ToBase64String(hash) == storedHash;
    }
}
