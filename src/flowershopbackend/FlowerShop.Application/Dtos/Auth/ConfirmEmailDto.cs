namespace FlowerShop.Application.Dtos.Auth;

/// <summary>Email confirmation request payload containing the raw token from the verification link.</summary>
public record ConfirmEmailDto(string Token);
