namespace FlowerShop.Application.Interfaces;

/// <summary>
/// Generates signed JSON Web Tokens for authenticated users.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Creates a signed JWT for the given user identity.
    /// </summary>
    /// <param name="userId">The user's primary key (stored as the <c>sub</c> claim).</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="name">The user's display name.</param>
    /// <param name="role">Optional role to include as a <c>role</c> claim (e.g. "Admin", "Customer").</param>
    /// <returns>A signed, base64url-encoded JWT string.</returns>
    string GenerateToken(long userId, string email, string name, string? role = null);
}
