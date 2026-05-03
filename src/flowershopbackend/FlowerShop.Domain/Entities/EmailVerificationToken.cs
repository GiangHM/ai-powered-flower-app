using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowerShop.Domain.Entities;

/// <summary>
/// Stores a hashed one-time token used to verify a user's email address.
/// </summary>
public class EmailVerificationToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>Foreign key to the owning <see cref="User"/>.</summary>
    public long UserId { get; set; }

    /// <summary>SHA-256 hash of the raw token that was sent to the user.</summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>UTC timestamp after which the token is no longer valid.</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>True once the token has been consumed during email confirmation.</summary>
    public bool IsUsed { get; set; }

    /// <summary>UTC timestamp when the token was created.</summary>
    public DateTime CreationDate { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
