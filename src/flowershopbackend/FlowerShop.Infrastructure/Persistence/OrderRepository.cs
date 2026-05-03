using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence
{
    public class OrderRepository : IOrderRepository
    {
        private readonly FlowerShopDbContext _context;

        public OrderRepository(FlowerShopDbContext context)
        {
            _context = context;
        }

        /// <summary>Gets an order by ID including its items.</summary>
        public async Task<Order?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        /// <summary>Gets all orders including their items.</summary>
        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ToListAsync(cancellationToken);
        }

        /// <summary>Adds a new order and returns the tracked entity.</summary>
        public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            var entry = await _context.Orders.AddAsync(order, cancellationToken);
            return entry.Entity;
        }

        /// <summary>Gets all orders for a specific user including their items.</summary>
        public async Task<IEnumerable<Order>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>Returns a page of orders, optionally filtered by status.</summary>
        public async Task<IEnumerable<Order>> GetPagedAsync(int page, int pageSize, OrderStatus? status, CancellationToken cancellationToken = default)
        {
            var query = _context.Orders.Include(o => o.Items).AsQueryable();
            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            return await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        /// <summary>Returns the total count of orders, optionally filtered by status.</summary>
        public async Task<int> CountAsync(OrderStatus? status, CancellationToken cancellationToken = default)
        {
            var query = _context.Orders.AsQueryable();
            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            return await query.CountAsync(cancellationToken);
        }

        /// <summary>Marks an existing order as modified so changes are saved on next commit.</summary>
        public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            _context.Orders.Update(order);
            return Task.CompletedTask;
        }
    }
}
