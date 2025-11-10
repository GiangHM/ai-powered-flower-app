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

        public async Task<IEnumerable<Flower>> GetActivatedFlowerAsync()
        {
            return await _context.Flowers
                .Where(f => f.IsActive)
                .Include(o => o.UnitPrice)
                .Include(o => o.Category)
                .ToListAsync();
        }
    }
}
