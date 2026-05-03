
using FlowerShop.Domain.Interfaces;

namespace FlowerShop.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly FlowerShopDbContext _context;

    public UnitOfWork(FlowerShopDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
