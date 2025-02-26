using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using AuthenticationServices.Requests;
using AuthenticationServices.Responses;
using DatabaseServices;
using DatabaseServices.Models;
using EmailServices;
using EmailServices.Requests;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using LoginRequest = AuthenticationServices.Requests.LoginRequest;

namespace AuthenticationServices;

public class AuthenticationService
{
    private readonly DatabaseService<User> _dbService;

    private readonly TokenService _tokenService;

    private readonly EmailService _emailService;

    private readonly JwtSettings _jwtSettings;
    
    public AuthenticationService(DatabaseService<User> dbService, TokenService tokenService, EmailService emailService, IOptions<JwtSettings> jwtSettings)
    {
        _dbService = dbService;
        _tokenService = tokenService;
        _emailService = emailService;
        _jwtSettings = jwtSettings.Value;
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

        user.Password = Hash(request.Password, salt);
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

        if (IsValidHash(request.Password, user.Password!, user.Salt!))
            response.Token = _tokenService.GenerateAccessToken(user.Id);

        return response;
    }

    public async Task<ChangePasswordResponse?> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var response = new ChangePasswordResponse();

        var filter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var user = await _dbService.FindOneAsync(filter).ConfigureAwait(false);
        if (user == null)
            return null;

        if (!IsValidHash(request.CurrentPassword, user.Password!, user.Salt!))
            return response;

        var newPasswordHash = Hash(request.NewPassword, Convert.FromBase64String(user.Salt!));

        var updateDef = Builders<User>.Update.Set(x => x.Password, newPasswordHash);
        var updateResponse = await _dbService.UpdateOneAsync(filter, updateDef).ConfigureAwait(false);
        response.ChangeSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }   

    public async Task<ForgotPasswordResponse?> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var response = new ForgotPasswordResponse();

        var emailFilter = Builders<User>.Filter.Eq(x => x.Email, request.Email);
        var user = await _dbService.FindOneAsync(emailFilter).ConfigureAwait(false);
        if (user == null)
            return null;

        var resetToken = _tokenService.GeneratePasswordResetToken(user.Id, request.Email);
        var resetTokenHash = Hash(resetToken, Convert.FromBase64String(user.Salt!));

        var updateFilter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
        var update = Builders<User>.Update.Set(x => x.PasswordResetToken, resetTokenHash)
                                          .Set(x => x.PasswordResetExpiration, DateTime.UtcNow.AddMinutes(int.Parse(_jwtSettings.AccessExpiryMinutes)));
        var updateResponse = await _dbService.UpdateOneAsync(updateFilter, update).ConfigureAwait(false);
        response.SendSuccess = updateResponse.ModifiedCount == 1;

        if (response.SendSuccess == true)
            await _emailService.SendPasswordResetEmail(request.Email, resetToken).ConfigureAwait(false);
        
        return response;
    }

    public async Task<ResetPasswordResponse?> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var response = new ResetPasswordResponse();

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(request.ResetToken);
        var emailClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;

        var filter = Builders<User>.Filter.Eq(x => x.Email, emailClaim);
        var user = await _dbService.FindOneAsync(filter).ConfigureAwait(false);
        if (user == null)
            return null;

        if (user.PasswordResetExpiration == null || user.PasswordResetExpiration < DateTime.UtcNow)
            return response;

        if (Hash(request.ResetToken, Convert.FromBase64String(user.Salt!)) != user.PasswordResetToken)
            return response;

        var newPasswordHash = Hash(request.NewPassword, Convert.FromBase64String(user.Salt!));

        var updateFilter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
        var update = Builders<User>.Update.Set(x => x.Password, newPasswordHash)
                                          .Unset(x => x.PasswordResetExpiration)
                                          .Unset(x => x.PasswordResetToken);
        var updateResponse = await _dbService.UpdateOneAsync(updateFilter, update).ConfigureAwait(false);
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
