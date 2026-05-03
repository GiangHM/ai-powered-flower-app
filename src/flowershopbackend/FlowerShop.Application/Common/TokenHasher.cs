using System.Security.Cryptography;
using System.Text;

namespace FlowerShop.Application.Common;

/// <summary>
/// Provides helpers for hashing one-time verification tokens.
/// The raw token is sent to the user; only its hash is stored in the database.
/// </summary>
public static class TokenHasher
{
    /// <summary>
    /// Computes a SHA-256 hash of <paramref name="rawToken"/> and returns it as a Base64 string.
    /// </summary>
    /// <param name="rawToken">The raw (un-hashed) token string.</param>
    /// <returns>A Base64-encoded SHA-256 hash of the token.</returns>
    public static string Hash(string rawToken)
        => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));
}
