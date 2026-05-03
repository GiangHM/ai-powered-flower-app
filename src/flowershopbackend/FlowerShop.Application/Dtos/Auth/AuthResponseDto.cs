namespace FlowerShop.Application.Dtos.Auth;

/// <summary>Returned to the caller after a successful register or login.</summary>
public record AuthResponseDto(string Token, string Email, string Name);
