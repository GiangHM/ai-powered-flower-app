using FlowerShop.Domain.Entities;

namespace FlowerShop.Domain.Interfaces
{
    public interface IOrderRepository
    {
        /// <summary>Gets an order by ID including its items.</summary>
        Task<Order?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>Gets all orders including their items.</summary>
        Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>Adds a new order and returns the tracked entity.</summary>
        Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);

        /// <summary>Gets all orders for a specific user including their items.</summary>
        Task<IEnumerable<Order>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>Returns a page of orders, optionally filtered by status.</summary>
        Task<IEnumerable<Order>> GetPagedAsync(int page, int pageSize, OrderStatus? status, CancellationToken cancellationToken = default);

        /// <summary>Returns the total count of orders, optionally filtered by status.</summary>
        Task<int> CountAsync(OrderStatus? status, CancellationToken cancellationToken = default);

        /// <summary>Marks an existing order as modified so changes are saved on next commit.</summary>
        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
    }
}
