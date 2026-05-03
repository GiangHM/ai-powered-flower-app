using FlowerShop.Application.Dtos.ChatFeature;
using FlowerShop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FlowerShop.Api.Controllers;

/// <summary>Exposes writer and sales agent streaming HTTP endpoints.</summary>
[Route("api/[controller]")]
[ApiController]
public class ChatController(
    IWriterAgentService writerAgentService,
    ISalesAgentService salesAgentService) : ControllerBase
{
    private static readonly JsonSerializerOptions _caseInsensitiveOptions =
        new() { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// Streams a writer-agent response as newline-delimited JSON (application/x-ndjson).
    /// Accepts an <see cref="AIChatRequest"/> compatible with the
    /// <c>@microsoft/ai-chat-protocol</c> package. The last user message must have its
    /// <c>content</c> set to a JSON object <c>{ "research": "...", "writing": "..." }</c>.
    /// </summary>
    /// <param name="request">AI Chat Protocol request containing the user messages.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A streaming HTTP response where each newline-delimited line is a serialised <see cref="AIChatCompletionDelta"/>.</returns>
    [HttpPost("writer/stream")]
    [Consumes("application/json")]
    public async Task StreamWriterAsync(
        [FromBody] AIChatRequest request,
        CancellationToken cancellationToken)
    {
        var lastUserMessage = request.Messages
            .LastOrDefault(m => m.Role == AIChatRole.User);

        if (lastUserMessage.Content is null)
            return;

        var writerInput = JsonSerializer.Deserialize<CreateWriterRequest>(
            lastUserMessage.Content, _caseInsensitiveOptions);

        if (writerInput is null)
            return;

        Response.Headers.Append("Content-Type", "application/x-ndjson");

        await foreach (var delta in writerAgentService.ProcessStreamingAsync(writerInput, cancellationToken))
        {
            await Response.WriteAsync($"{JsonSerializer.Serialize(delta)}\r\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Streams a sales-agent response as newline-delimited JSON (application/x-ndjson).
    /// The agent uses function-calling tools to search for flowers, retrieve pricing,
    /// and place orders on behalf of the customer.
    /// Conversation history is maintained server-side per <c>sessionId</c>.
    /// </summary>
    /// <param name="request">
    /// Sales chat request containing <c>sessionId</c>, the user <c>message</c>,
    /// and an optional <c>history</c> array to seed a new session.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A streaming HTTP response where each newline-delimited line is a serialised <see cref="AIChatCompletionDelta"/>.</returns>
    [HttpPost("sales/stream")]
    [Consumes("application/json")]
    [Produces("application/x-ndjson")]
    public async Task StreamSalesAsync(
        [FromBody] SalesChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId) || string.IsNullOrWhiteSpace(request.Message))
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            await Response.WriteAsync("sessionId and message are required.", cancellationToken);
            return;
        }

        Response.Headers.Append("Content-Type", "application/x-ndjson");

        await foreach (var delta in salesAgentService.StreamSalesResponseAsync(request, cancellationToken))
        {
            await Response.WriteAsync($"{JsonSerializer.Serialize(delta)}\r\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
