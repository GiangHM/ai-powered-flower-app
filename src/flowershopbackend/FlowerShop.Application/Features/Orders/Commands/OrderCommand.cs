using FlowerShop.Application.Common;
using FlowerShop.Application.Dtos;
using FlowerShop.Application.Features.Orders.Queries;
using FlowerShop.Application.Interfaces;
using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;

namespace FlowerShop.Application.Features.Orders.Commands
{
    public interface IPlaceOrderCommand<I, R>
    {
        Task<R> Handle(I request, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Validates stock, deducts quantities, creates the order, and persists all changes atomically.
    /// </summary>
    public class PlaceOrderCommand : IPlaceOrderCommand<CreateOrderDto, Result<OrderResponseDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IFlowerResponsitory _flowerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PlaceOrderCommand(
            IOrderRepository orderRepository,
            IFlowerResponsitory flowerRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _flowerRepository = flowerRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>Places a new order after validating and deducting stock for each item.</summary>
        public async Task<Result<OrderResponseDto>> Handle(CreateOrderDto request, CancellationToken cancellationToken)
        {
            var deliveryInfoResult = await ResolveDeliveryInfoAsync(request, cancellationToken);
            if (!deliveryInfoResult.IsSuccess)
                return Result<OrderResponseDto>.Failure(deliveryInfoResult.Error!);

            var deliveryInfo = deliveryInfoResult.Value!;

            var flowerIds = request.Items.Select(i => i.FlowerId).Distinct();
            var flowers = (await _flowerRepository.GetByIdsWithStockAsync(flowerIds)).ToDictionary(f => f.Id);

            foreach (var item in request.Items)
            {
                if (!flowers.TryGetValue(item.FlowerId, out var flower))
                    return Result<OrderResponseDto>.Failure($"Flower with ID {item.FlowerId} not found.");

                if (flower.Stock == null || flower.Stock.Quantity < item.Quantity)
                    return Result<OrderResponseDto>.Failure($"Insufficient stock for flower '{flower.FlowerName}'.");
            }

            foreach (var item in request.Items)
            {
                var flower = flowers[item.FlowerId];
                flower.Stock!.UpdateStock(item.Quantity, flower.Stock.QuantityUnit, StockUpdateMode.Decrease);
                await _flowerRepository.UpdateAsync(flower);
            }

            var orderItemData = request.Items.Select(i => (
                FlowerId: i.FlowerId,
                FlowerName: flowers[i.FlowerId].FlowerName,
                Quantity: i.Quantity,
                UnitPrice: flowers[i.FlowerId].UnitPrice?.Price.Amount ?? 0m
            ));

            var order = Order.PlaceOrder(
                request.UserId,
                deliveryInfo.Name,
                deliveryInfo.Email,
                deliveryInfo.Phone,
                orderItemData);
            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<OrderResponseDto>.Success(MapToResponse(order));
        }

        private static OrderResponseDto MapToResponse(Order order) => new()
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

        private async Task<Result<DeliveryInfo>> ResolveDeliveryInfoAsync(CreateOrderDto request, CancellationToken cancellationToken)
        {
            if (request.UserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);
                if (user == null)
                    return Result<DeliveryInfo>.Failure($"User with ID {request.UserId.Value} not found.");

                return Result<DeliveryInfo>.Success(new DeliveryInfo(user.Name, user.Email, user.Phone));
            }

            if (string.IsNullOrWhiteSpace(request.DeliveryName))
                return Result<DeliveryInfo>.Failure("Delivery name is required for guest checkout.");
            if (string.IsNullOrWhiteSpace(request.DeliveryEmail))
                return Result<DeliveryInfo>.Failure("Delivery email is required for guest checkout.");
            if (string.IsNullOrWhiteSpace(request.DeliveryPhone))
                return Result<DeliveryInfo>.Failure("Delivery phone is required for guest checkout.");

            return Result<DeliveryInfo>.Success(new DeliveryInfo(
                request.DeliveryName.Trim(),
                request.DeliveryEmail.Trim(),
                request.DeliveryPhone.Trim()));
        }

        private sealed record DeliveryInfo(string Name, string Email, string Phone);
    }

    /// <summary>Contract for updating an order's status (admin use).</summary>
    public interface IUpdateOrderStatusCommand<TIn, TOut>
    {
        Task<TOut> Handle(long orderId, TIn request, CancellationToken cancellationToken = default);
    }

    /// <summary>Updates the status of an existing order.</summary>
    public class UpdateOrderStatusCommand : IUpdateOrderStatusCommand<UpdateOrderStatusDto, Result<OrderResponseDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOrderStatusCommand(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>Handles the order status update request.</summary>
        public async Task<Result<OrderResponseDto>> Handle(
            long orderId,
            UpdateOrderStatusDto request,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order is null)
                return Result<OrderResponseDto>.Failure($"Order with ID {orderId} not found.");

            if (!Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var newStatus))
                return Result<OrderResponseDto>.Failure(
                    $"Invalid status value '{request.Status}'. Valid values: {string.Join(", ", Enum.GetNames<OrderStatus>())}.");

            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<OrderResponseDto>.Success(OrderDtoMapper.MapToDto(order));
        }
    }
}
