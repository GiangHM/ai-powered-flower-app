using FlowerShop.Application.Common;
using FlowerShop.Application.Dtos;
using FlowerShop.Application.Features.Orders.Commands;
using FlowerShop.Application.Features.Orders.Queries;
using FlowerShop.Application.Interfaces;

namespace FlowerShop.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IPlaceOrderCommand<CreateOrderDto, Result<OrderResponseDto>> _placeOrderCommand;
        private readonly IGetOrderByIdQuery<Result<OrderResponseDto>> _getOrderByIdQuery;
        private readonly IGetAllOrdersQuery<IEnumerable<OrderResponseDto>> _getAllOrdersQuery;

        public OrderService(
            IPlaceOrderCommand<CreateOrderDto, Result<OrderResponseDto>> placeOrderCommand,
            IGetOrderByIdQuery<Result<OrderResponseDto>> getOrderByIdQuery,
            IGetAllOrdersQuery<IEnumerable<OrderResponseDto>> getAllOrdersQuery)
        {
            _placeOrderCommand = placeOrderCommand;
            _getOrderByIdQuery = getOrderByIdQuery;
            _getAllOrdersQuery = getAllOrdersQuery;
        }

        /// <summary>Places a new order.</summary>
        public Task<Result<OrderResponseDto>> PlaceOrderAsync(CreateOrderDto request, CancellationToken cancellationToken = default)
            => _placeOrderCommand.Handle(request, cancellationToken);

        /// <summary>Gets an order by ID.</summary>
        public Task<Result<OrderResponseDto>> GetOrderByIdAsync(long id, CancellationToken cancellationToken = default)
            => _getOrderByIdQuery.Handle(id, cancellationToken);

        /// <summary>Gets all orders.</summary>
        public Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
            => _getAllOrdersQuery.Handle(cancellationToken);
    }
}
