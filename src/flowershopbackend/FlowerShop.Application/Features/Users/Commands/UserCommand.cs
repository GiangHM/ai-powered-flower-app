using FlowerShop.Application.Common;
using FlowerShop.Application.Dtos;
using FlowerShop.Application.Interfaces;
using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;

namespace FlowerShop.Application.Features.Users.Commands;

/// <summary>Contract for updating a user's account status.</summary>
public interface IUpdateUserStatusCommand<TIn, TOut>
{
    Task<TOut> Handle(long userId, TIn request, CancellationToken cancellationToken = default);
}

/// <summary>Suspends or reactivates a user account by updating their status.</summary>
public class UpdateUserStatusCommand : IUpdateUserStatusCommand<UpdateUserStatusDto, Result<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserStatusCommand(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>Handles the status update request.</summary>
    public async Task<Result<UserResponseDto>> Handle(
        long userId,
        UpdateUserStatusDto request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return Result<UserResponseDto>.Failure($"User with ID {userId} not found.");

        if (!Enum.TryParse<UserStatus>(request.Status, ignoreCase: true, out var newStatus))
            return Result<UserResponseDto>.Failure($"Invalid status value '{request.Status}'.");

        user.Status = newStatus;
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserResponseDto>.Success(UserDtoMapper.MapToDto(user));
    }
}

/// <summary>Contract for editing a user's basic details.</summary>
public interface IUpdateUserCommand<TIn, TOut>
{
    Task<TOut> Handle(long userId, TIn request, CancellationToken cancellationToken = default);
}

/// <summary>Updates a user's name, phone, and delivery address.</summary>
public class UpdateUserCommand : IUpdateUserCommand<UpdateUserDto, Result<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommand(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>Handles the user details update request.</summary>
    public async Task<Result<UserResponseDto>> Handle(
        long userId,
        UpdateUserDto request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return Result<UserResponseDto>.Failure($"User with ID {userId} not found.");

        user.Name = request.Name;
        user.Phone = request.Phone;
        user.DeliveryAddress = request.DeliveryAddress;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserResponseDto>.Success(UserDtoMapper.MapToDto(user));
    }
}

/// <summary>Internal helper to map a <see cref="User"/> to a <see cref="UserResponseDto"/>.</summary>
internal static class UserDtoMapper
{
    internal static UserResponseDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Phone = user.Phone,
        Email = user.Email,
        DeliveryAddress = user.DeliveryAddress,
        Status = user.Status.ToString(),
        EmailVerified = user.EmailVerified,
        CreationDate = user.CreationDate,
        Role = user.Role
    };
}
