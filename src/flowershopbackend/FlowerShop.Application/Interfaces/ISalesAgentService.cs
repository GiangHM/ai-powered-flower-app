using FlowerShop.Application.Dtos.ChatFeature;

namespace FlowerShop.Application.Interfaces;

/// <summary>
/// Sales agent that helps customers discover flowers, get pricing, and place orders
/// via a streaming chat interface backed by AI function-calling tools.
/// </summary>
public interface ISalesAgentService
{
    /// <summary>
    /// Streams the sales agent's response for the given chat request using Server-Sent Events.
    /// Conversation history is maintained server-side per <see cref="SalesChatRequest.SessionId"/>.
    /// </summary>
    /// <param name="request">The chat request containing the session ID, user message, and optional prior history.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async stream of chat completion deltas from the sales agent.</returns>
    IAsyncEnumerable<AIChatCompletionDelta> StreamSalesResponseAsync(
        SalesChatRequest request,
        CancellationToken cancellationToken = default);
}
