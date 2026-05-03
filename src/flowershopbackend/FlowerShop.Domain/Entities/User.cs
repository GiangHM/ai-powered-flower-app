using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowerShop.Domain.Entities;

/// <summary>
/// Represents a registered user in the FlowerShop system.
/// </summary>
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>Full display name of the user.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Contact phone number.</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Unique email address used for login and notifications.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Salted SHA-256 hash of the user's password (format: salt:hash).</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Optional default delivery address.</summary>
    public string? DeliveryAddress { get; set; }

    /// <summary>Current account status.</summary>
    public UserStatus Status { get; set; } = UserStatus.Pending;

    /// <summary>Whether the user has completed email verification.</summary>
    public bool EmailVerified { get; set; }

    /// <summary>UTC timestamp when the user account was created.</summary>
    public DateTime CreationDate { get; set; }

    /// <summary>Role assigned to this account: "Admin" or "Customer". Null defaults to "Customer" on login.</summary>
    public string? Role { get; set; }

    /// <summary>Email verification tokens associated with this user.</summary>
    public ICollection<EmailVerificationToken> VerificationTokens { get; set; } = new List<EmailVerificationToken>();
}
