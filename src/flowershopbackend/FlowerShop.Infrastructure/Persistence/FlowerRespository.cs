using Azure.Core;
using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence
{
    public class FlowerRespository : IFlowerResponsitory
    {
        private readonly FlowerShopDbContext _context;
        public FlowerRespository(FlowerShopDbContext context)
        {
            _context = context;
        }
        public async Task<Flower> AddAsync(Flower item)
        {
            var entry = await _context.Flowers.AddAsync(item);
            return entry.Entity;
        }

        public async Task DeleteAsync(long id)
        {
            var order = await GetByIdAsync(id);
            if (order != null)
            {
                _context.Flowers.Remove(order);
            }
        }

        public async Task<IEnumerable<Flower>> GetAllAsync()
        {
            return await _context.Flowers
                .Include(o => o.UnitPrice)
                .Include(o => o.Category)
                .ToListAsync();
        }
        public async Task<IEnumerable<Flower>> GetByIdsAsync(IEnumerable<long> ids)
        {
            return await _context.Flowers
                .Where(f => ids.Contains(f.Id))
                .Include(o => o.UnitPrice)
                .ToListAsync();
        }
        public async Task<Flower?> GetByIdAsync(long id)
        {
            return await _context.Flowers
                .Include(o => o.UnitPrice)
                .Include(o => o.Category)
                .Include(o => o.Stock)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task UpdateAsync(Flower item)
        {
            ArgumentNullException.ThrowIfNull(item);
            _context.Flowers.Update(item);
            await Task.CompletedTask;
        }
        public async Task<IEnumerable<Flower>> SearchAsync(string keyword)
        {
            return await _context.Flowers
                .Where(f => string.IsNullOrEmpty(keyword) ||
                            EF.Functions.Like(f.FlowerName, $"%{keyword}%") )
                .Include(o => o.UnitPrice)
                .ToListAsync();
        }

        public async Task<IEnumerable<Flower>> GetByIdsWithStockAsync(IEnumerable<long> ids)
        {
            return await _context.Flowers
                .Where(f => ids.Contains(f.Id))
                .Include(o => o.UnitPrice)
                .Include(o => o.Stock)
                .ToListAsync();
        }

        public async Task<IEnumerable<Flower>> GetActivatedFlowerAsync()
        {
            return await _context.Flowers
                .Where(f => f.IsActive && f.Stock != null && f.Stock.Quantity > 0)
                .Include(o => o.UnitPrice)
                .Include(o => o.Category)
                .Include(o => o.Stock)
                .ToListAsync();
        }

        public async Task<IEnumerable<Flower>> GetActivatedFlowerPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Flowers
                .Where(f => f.IsActive && f.Stock != null && f.Stock.Quantity > 0)
                .Include(o => o.UnitPrice)
                .Include(o => o.Category)
                .Include(o => o.Stock)
                .OrderBy(f => f.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountActivatedFlowersAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Flowers
                .CountAsync(f => f.IsActive && f.Stock != null && f.Stock.Quantity > 0, cancellationToken);
        }
    }
}
