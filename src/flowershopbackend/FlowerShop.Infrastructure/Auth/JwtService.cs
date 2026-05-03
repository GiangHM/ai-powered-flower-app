using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlowerShop.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FlowerShop.Infrastructure.Auth;

/// <summary>
/// Generates signed JWT access tokens using HMAC-SHA256.
/// </summary>
public class JwtService : IJwtService
{
    private readonly JwtOptions _options;

    public JwtService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Creates a signed JWT for the given user identity.
    /// </summary>
    /// <param name="userId">User primary key stored as the <c>sub</c> claim.</param>
    /// <param name="email">User email stored as the <c>email</c> claim.</param>
    /// <param name="name">User display name stored as the <c>name</c> claim.</param>
    /// <param name="role">Optional role stored as the <c>role</c> claim (e.g. "Admin", "Customer").</param>
    /// <returns>A compact, base64url-encoded JWT string.</returns>
    public string GenerateToken(long userId, string email, string name, string? role = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Name, name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrWhiteSpace(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
