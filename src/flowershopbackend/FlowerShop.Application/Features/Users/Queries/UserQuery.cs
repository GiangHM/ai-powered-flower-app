using FlowerShop.Application.Common;
using FlowerShop.Application.Dtos;
using FlowerShop.Application.Interfaces;
using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;

namespace FlowerShop.Application.Features.Users.Queries;

/// <summary>Contract for retrieving a paginated list of users.</summary>
public interface IGetUsersPagedQuery<TOut>
{
    Task<TOut> Handle(int page, int pageSize, UserStatus? status, CancellationToken cancellationToken = default);
}

/// <summary>Returns a paginated list of users, optionally filtered by status.</summary>
public class GetUsersPagedQuery : IGetUsersPagedQuery<PagedResult<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersPagedQuery(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>Handles the paginated user list query.</summary>
    public async Task<PagedResult<UserResponseDto>> Handle(
        int page,
        int pageSize,
        UserStatus? status,
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetPagedAsync(page, pageSize, status, cancellationToken);
        var total = await _userRepository.CountAsync(status, cancellationToken);

        return new PagedResult<UserResponseDto>
        {
            Items = users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Phone = u.Phone,
                Email = u.Email,
                DeliveryAddress = u.DeliveryAddress,
                Status = u.Status.ToString(),
                EmailVerified = u.EmailVerified,
                CreationDate = u.CreationDate,
                Role = u.Role
            }),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }
}

/// <summary>Contract for retrieving a user's order history.</summary>
public interface IGetUserOrdersQuery<TOut>
{
    Task<TOut> Handle(long userId, CancellationToken cancellationToken = default);
}

/// <summary>Returns all orders for a given user, or a failure result when the user is not found.</summary>
public class GetUserOrdersQuery : IGetUserOrdersQuery<Result<IEnumerable<OrderResponseDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;

    public GetUserOrdersQuery(IUserRepository userRepository, IOrderRepository orderRepository)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
    }

    /// <summary>Handles the user orders query.</summary>
    public async Task<Result<IEnumerable<OrderResponseDto>>> Handle(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return Result<IEnumerable<OrderResponseDto>>.Failure($"User with ID {userId} not found.");

        var orders = await _orderRepository.GetByUserIdAsync(userId, cancellationToken);

        var dtos = orders.Select(order => new OrderResponseDto
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
        });

        return Result<IEnumerable<OrderResponseDto>>.Success(dtos);
    }
}
