namespace FlowerShop.Domain.Entities;

/// <summary>
/// Represents the lifecycle state of a user account.
/// </summary>
public enum UserStatus
{
    /// <summary>Registered but email not yet verified.</summary>
    Pending = 0,

    /// <summary>Email verified; full access granted.</summary>
    Active = 1,

    /// <summary>Account has been deactivated.</summary>
    Inactive = 2
}
