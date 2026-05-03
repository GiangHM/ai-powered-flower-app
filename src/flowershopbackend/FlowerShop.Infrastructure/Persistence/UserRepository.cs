using FlowerShop.Application.Interfaces;
using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of <see cref="IUserRepository"/>.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly FlowerShopDbContext _context;

    public UserRepository(FlowerShopDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Finds a user by their email address.
    /// Case sensitivity depends on the database collation (case-insensitive by default in SQL Server).
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <summary>Finds a user by primary key.</summary>
    public async Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <summary>Persists a new user and returns the tracked entity.</summary>
    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Users.AddAsync(user, cancellationToken);
        return entry.Entity;
    }

    /// <summary>Marks an existing user as modified.</summary>
    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves a verification token by its stored hash, eager-loading the associated user.
    /// </summary>
    public async Task<EmailVerificationToken?> GetVerificationTokenAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmailVerificationTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
    }

    /// <summary>Persists a new verification token.</summary>
    public async Task AddVerificationTokenAsync(
        EmailVerificationToken token,
        CancellationToken cancellationToken = default)
    {
        await _context.EmailVerificationTokens.AddAsync(token, cancellationToken);
    }

    /// <summary>Returns a page of users, optionally filtered by status.</summary>
    public async Task<IEnumerable<User>> GetPagedAsync(
        int page,
        int pageSize,
        UserStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();
        if (status.HasValue)
            query = query.Where(u => u.Status == status.Value);

        return await query
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>Returns the total count of users, optionally filtered by status.</summary>
    public async Task<int> CountAsync(
        UserStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();
        if (status.HasValue)
            query = query.Where(u => u.Status == status.Value);

        return await query.CountAsync(cancellationToken);
    }
}
