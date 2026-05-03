using FlowerShop.Application.Common;
using FlowerShop.Application.Dtos;

namespace FlowerShop.Application.Interfaces
{
    public interface IOrderService
    {
        /// <summary>Places a new order. Returns failure if stock is insufficient.</summary>
        Task<Result<OrderResponseDto>> PlaceOrderAsync(CreateOrderDto request, CancellationToken cancellationToken = default);

        /// <summary>Gets an order by ID. Returns failure if the order does not exist.</summary>
        Task<Result<OrderResponseDto>> GetOrderByIdAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>Gets all orders for the admin view.</summary>
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    }
}
