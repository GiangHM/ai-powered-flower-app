using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application.Dtos;

/// <summary>Represents a page of results with total-count metadata.</summary>
/// <typeparam name="T">The item type.</typeparam>
public class PagedResult<T>
{
    /// <summary>Items in the current page.</summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>Total number of items matching the filter (across all pages).</summary>
    public int TotalCount { get; set; }

    /// <summary>Current page number (1-based).</summary>
    public int Page { get; set; }

    /// <summary>Maximum items per page.</summary>
    public int PageSize { get; set; }
}

/// <summary>User summary returned by the admin list endpoint.</summary>
public class UserResponseDto
{
    /// <summary>User primary key.</summary>
    public long Id { get; set; }

    /// <summary>Full display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Contact phone number.</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Unique email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Optional default delivery address.</summary>
    public string? DeliveryAddress { get; set; }

    /// <summary>Current account status as a string (e.g. "Active", "Inactive").</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Whether the user's email has been verified.</summary>
    public bool EmailVerified { get; set; }

    /// <summary>UTC timestamp when the account was created.</summary>
    public DateTime CreationDate { get; set; }

    /// <summary>Role assigned to this account (e.g. "Admin", "Customer").</summary>
    public string? Role { get; set; }
}

/// <summary>Payload for updating basic user details (admin use).</summary>
public class UpdateUserDto
{
    /// <summary>Updated full name.</summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Updated phone number.</summary>
    [Required]
    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    /// <summary>Updated default delivery address.</summary>
    [MaxLength(500)]
    public string? DeliveryAddress { get; set; }
}

/// <summary>Payload for updating a user's account status (admin use).</summary>
public class UpdateUserStatusDto
{
    /// <summary>
    /// New status for the user account.
    /// Use <c>Active</c> to reactivate and <c>Inactive</c> to suspend.
    /// </summary>
    [Required]
    public string Status { get; set; } = string.Empty;
}
