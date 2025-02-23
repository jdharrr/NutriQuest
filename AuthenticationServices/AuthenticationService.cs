using System.Security.Cryptography;
using System.Text;
using DatabaseServices;
using DatabaseServices.Models;

namespace AuthenticationServices;

public class AuthenticationService
{
    private readonly DatabaseService<User> _dbService;
    
    public AuthenticationService(DatabaseService<User> dbService)
    {
        _dbService = dbService;
    }

    public async Task CreateNewUser(string email, string password)
    {
        var user = new User
        {
            Email = email
        };
        
        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);

        user.Password = HashPassword(password, salt);
        user.Salt = Convert.ToBase64String(salt);

        await _dbService.InsertOneAsync(user);
    }

    private string HashPassword(string password, byte[] salt)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var combinedBytes = new byte[salt.Length + passwordBytes.Length];
        Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
        Buffer.BlockCopy(passwordBytes, 0, combinedBytes, salt.Length, passwordBytes.Length);
        
        return Convert.ToBase64String(SHA256.HashData(combinedBytes));
    }
}
