using FlowerShop.Application.Common;
using FlowerShop.Application.Dtos;
using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;

namespace FlowerShop.Application.Features.Orders.Queries
{
    public interface IGetOrderByIdQuery<R>
    {
        Task<R> Handle(long id, CancellationToken cancellationToken = default);
    }

    /// <summary>Retrieves a single order by ID including its items.</summary>
    public class GetOrderByIdQuery : IGetOrderByIdQuery<Result<OrderResponseDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQuery(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>Returns the order, or a failure result when the order is not found.</summary>
        public async Task<Result<OrderResponseDto>> Handle(long id, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
            if (order == null)
                return Result<OrderResponseDto>.Failure($"Order with ID {id} not found.");

            return Result<OrderResponseDto>.Success(OrderDtoMapper.MapToDto(order));
        }
    }

    public interface IGetAllOrdersQuery<R>
    {
        Task<R> Handle(CancellationToken cancellationToken);
    }

    /// <summary>Retrieves all orders for admin use, including items.</summary>
    public class GetAllOrdersQuery : IGetAllOrdersQuery<IEnumerable<OrderResponseDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetAllOrdersQuery(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>Returns all orders with their line items.</summary>
        public async Task<IEnumerable<OrderResponseDto>> Handle(CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetAllAsync(cancellationToken);
            return orders.Select(OrderDtoMapper.MapToDto);
        }
    }

    /// <summary>Contract for retrieving a paginated list of orders.</summary>
    public interface IGetOrdersPagedQuery<TOut>
    {
        Task<TOut> Handle(int page, int pageSize, OrderStatus? status, CancellationToken cancellationToken = default);
    }

    /// <summary>Returns a paginated list of orders, optionally filtered by status.</summary>
    public class GetOrdersPagedQuery : IGetOrdersPagedQuery<PagedResult<OrderResponseDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersPagedQuery(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>Handles the paginated order list query.</summary>
        public async Task<PagedResult<OrderResponseDto>> Handle(
            int page,
            int pageSize,
            OrderStatus? status,
            CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetPagedAsync(page, pageSize, status, cancellationToken);
            var total = await _orderRepository.CountAsync(status, cancellationToken);

            return new PagedResult<OrderResponseDto>
            {
                Items = orders.Select(OrderDtoMapper.MapToDto),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }

    /// <summary>Internal helper to map an <see cref="Order"/> to an <see cref="OrderResponseDto"/>.</summary>
    internal static class OrderDtoMapper
    {
        internal static OrderResponseDto MapToDto(Order order) => new()
        {
            Id = order.Id,
            UserId = order.UserId,
            DeliveryName = order.DeliveryName,
            DeliveryEmail = order.DeliveryEmail,
            DeliveryPhone = order.DeliveryPhone,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            Items = order.Items.Select(i => new OrderItemResponseDto
            {
                Id = i.Id,
                FlowerId = i.FlowerId,
                FlowerName = i.FlowerName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}
