using FlowerShop.Domain.Entities;

namespace FlowerShop.Application.Interfaces;

/// <summary>
/// Data access contract for <see cref="User"/> and its verification tokens.
/// </summary>
public interface IUserRepository
{
    /// <summary>Finds a user by their email address.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Finds a user by their primary key.</summary>
    Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>Persists a new user and returns the tracked entity.</summary>
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>Marks an existing user as modified so changes are saved on next commit.</summary>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>Retrieves an unused verification token by its stored hash.</summary>
    Task<EmailVerificationToken?> GetVerificationTokenAsync(string tokenHash, CancellationToken cancellationToken = default);

    /// <summary>Persists a new verification token.</summary>
    Task AddVerificationTokenAsync(EmailVerificationToken token, CancellationToken cancellationToken = default);

    /// <summary>Returns a page of users, optionally filtered by status.</summary>
    Task<IEnumerable<User>> GetPagedAsync(int page, int pageSize, UserStatus? status, CancellationToken cancellationToken = default);

    /// <summary>Returns the total count of users, optionally filtered by status.</summary>
    Task<int> CountAsync(UserStatus? status, CancellationToken cancellationToken = default);
}
