using System.Text.Json.Serialization;

namespace FlowerShop.Application.Dtos.ChatFeature;

/// <summary>Request payload for the sales chat streaming endpoint.</summary>
public record SalesChatRequest
{
    /// <summary>Unique session identifier used to maintain per-session conversation history.</summary>
    [JsonPropertyName("sessionId")]
    public string SessionId { get; init; } = string.Empty;

    /// <summary>The user's current message to the sales agent.</summary>
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Optional prior conversation history sent by the client.
    /// When provided and no server-side session exists yet, this seeds the session history.
    /// </summary>
    [JsonPropertyName("history")]
    public IList<AIChatMessage>? History { get; init; }
}
