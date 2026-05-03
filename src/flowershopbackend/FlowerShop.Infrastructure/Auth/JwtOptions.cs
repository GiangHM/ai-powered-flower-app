namespace FlowerShop.Infrastructure.Auth;

/// <summary>
/// Configuration options for JWT generation.
/// Bind from the <c>"Jwt"</c> section in <c>appsettings.json</c>.
/// </summary>
public class JwtOptions
{
    /// <summary>HMAC-SHA256 signing key (minimum 32 characters recommended).</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Token issuer claim value.</summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>Token audience claim value.</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>Number of minutes before the token expires. Defaults to 60.</summary>
    public int ExpiryMinutes { get; set; } = 60;
}
