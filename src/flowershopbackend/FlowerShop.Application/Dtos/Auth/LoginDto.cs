namespace FlowerShop.Application.Dtos.Auth;

/// <summary>Login request payload.</summary>
public record LoginDto(string Email, string Password);
