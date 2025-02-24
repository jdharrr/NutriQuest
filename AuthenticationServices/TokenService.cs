using DatabaseServices;
using DatabaseServices.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationServices;

public class TokenService
{
	private readonly DatabaseService<User> _dbService;

	private readonly string _secret;

	private readonly string _issuer;

	private readonly string _audience;

	private readonly int _expiryMinutes;

	public TokenService(DatabaseService<User> dbService, IOptions<JwtSettings> settings)
    {
		_dbService = dbService;
		_secret = settings.Value.Secret;
		_issuer = settings.Value.Issuer;
		_audience = settings.Value.Audience;
		_expiryMinutes = int.Parse(settings.Value.ExpiryMinutes);
    }

    public string GenerateToken(string userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames. Sub, userId)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
