using System.Security.Cryptography;
using System.Text;
using AuthenticationServices.Requests;
using DatabaseServices;
using DatabaseServices.Models;
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

    public async Task<string?> LoginAsync(LoginRequest request)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Email, request.Email);
        var user = await _dbService.FindOneAsync(filter).ConfigureAwait(false);
        if (user == null)
            return null;

        if (!IsValidPassword(request.Password, user.Password!, user.Salt!))
            return null;

        return _tokenService.GenerateToken(user.Id);
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
