using System.Security.Cryptography;
using System.Text;
using AuthenticationServices.Requests;
using AuthenticationServices.Responses;
using DatabaseServices;
using DatabaseServices.Models;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;

namespace AuthenticationServices;

public class AuthenticationService
{
    private readonly DatabaseService<User> _dbService;

    private readonly TokenService _tokenService;
    
    public AuthenticationService(DatabaseService<User> dbService, TokenService tokenService)
    {
        _dbService = dbService;
        _tokenService = tokenService;
    }

    public async Task<bool> CreateNewUserAsync(NewUserRequest request)
    {
        var existsFilter = Builders<User>.Filter.Eq(x => x.Email, request.Email);
        var existingUser = await _dbService.FindOneAsync(existsFilter).ConfigureAwait(false);
        if (existingUser != null)
            return false;

        var user = new User
        {
            Email = request.Email
        };
        
        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);

        user.Password = HashPassword(request.Password, salt);
        user.Salt = Convert.ToBase64String(salt);

        await _dbService.InsertOneAsync(user).ConfigureAwait(false);

        return true;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var response = new LoginResponse();

        var filter = Builders<User>.Filter.Eq(x => x.Email, request.Email);
        var user = await _dbService.FindOneAsync(filter).ConfigureAwait(false);
        if (user == null)
            return null;

        if (IsValidPassword(request.Password, user.Password!, user.Salt!))
            response.Token = _tokenService.GenerateToken(user.Id);

        return response;
    }

    public async Task<ChangePasswordResponse?> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var response = new ChangePasswordResponse();

        var filter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var user = await _dbService.FindOneAsync(filter).ConfigureAwait(false);
        if (user == null)
            return null;

        if (!IsValidPassword(request.CurrentPassword, user.Password!, user.Salt!))
            return response;

        var newPasswordHash = HashPassword(request.NewPassword, Convert.FromBase64String(user.Salt!));

        var updateDef = Builders<User>.Update.Set(x => x.Password, newPasswordHash);
        var updateResponse = await _dbService.UpdateOneAsync(filter, updateDef).ConfigureAwait(false);
        if (updateResponse.ModifiedCount != 1)
            response.ChangeSuccess = false;

        response.ChangeSuccess = true;

        return response;
    }   

    private static string HashPassword(string password, byte[] salt)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var combinedBytes = new byte[salt.Length + passwordBytes.Length];
        Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
        Buffer.BlockCopy(passwordBytes, 0, combinedBytes, salt.Length, passwordBytes.Length);
        
        return Convert.ToBase64String(SHA256.HashData(combinedBytes));
    }

    private static bool IsValidPassword(string givenPassword, string userPasswordHash, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        byte[] givenPasswordBytes = Encoding.UTF8.GetBytes(givenPassword);
        byte[] combineBytes = new byte[saltBytes.Length + givenPasswordBytes.Length];

        Buffer.BlockCopy(saltBytes, 0, combineBytes, 0, saltBytes.Length);
        Buffer.BlockCopy(givenPasswordBytes, 0, combineBytes, saltBytes.Length, givenPasswordBytes.Length);

        byte[] hash = SHA256.HashData(combineBytes);
        
        return Convert.ToBase64String(hash) == userPasswordHash;
    }
}
