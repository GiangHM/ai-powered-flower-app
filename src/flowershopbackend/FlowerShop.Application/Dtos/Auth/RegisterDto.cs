namespace FlowerShop.Application.Dtos.Auth;

/// <summary>Registration request payload.</summary>
public record RegisterDto(
    string Name,
    string Phone,
    string Email,
    string Password,
    string? DeliveryAddress);
