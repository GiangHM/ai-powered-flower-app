using FlowerShop.Application.Dtos;
using FlowerShop.Application.Dtos.ChatFeature;
using FlowerShop.Application.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace FlowerShop.Infrastructure.Agent;

/// <summary>
/// Sales agent that uses function-calling tools to help customers find flowers,
/// get pricing details, and place orders. Conversation history is maintained
/// server-side per session using an in-memory store.
/// </summary>
public class SalesAgentService(
    [FromKeyedServices("salesclient")] IChatClient chatClient,
    IAiSearchService aiSearchService,
    IFlowerService flowerService,
    IOrderService orderService,
    ILogger<SalesAgentService> logger) : ISalesAgentService
{
    private const string SystemPrompt =
        """
        You are a friendly and knowledgeable flower sales assistant for an online flower shop.
        Your goal is to help customers find the perfect flowers for any occasion, answer care
        questions, and assist with placing orders.

        Guidelines:
        - Suggest flowers based on the customer's occasion, budget, or preferences using the SearchFlowersByOccasion tool.
        - Use GetFlowerDetails to retrieve accurate pricing and stock information before recommending specific flowers.
        - Be warm, helpful, and knowledgeable about flowers and their meanings.
        - When a customer shows interest in purchasing, summarise the order details clearly (flower names, quantities, delivery information) and ask for explicit confirmation before placing the order.
        - Only call PlaceOrder after the customer has explicitly confirmed the order details.
        - If you asked for confirmation and the customer hasn't responded, gently remind them or offer alternative suggestions.
        - Always reply in a concise and friendly tone.
        """;

    /// <summary>
    /// Per-session entry holding the conversation history and its own lock object.
    /// Using a dedicated lock per session avoids contention between concurrent sessions.
    /// NOTE: This in-memory store is intentionally simple for this learning project.
    /// For production use, replace with a distributed cache (e.g., IDistributedCache backed
    /// by Redis) that supports sliding expiration to avoid unbounded memory growth.
    /// </summary>
    private sealed record SessionEntry(List<ChatMessage> History, object Lock);

    private static readonly ConcurrentDictionary<string, SessionEntry> _sessions = new();

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    // Tools are built once per service instance. Because SalesAgentService is transient,
    // each request gets fresh tool instances that capture the request-scoped services.
    private readonly List<AITool> _tools = BuildTools(aiSearchService, flowerService, orderService, logger);

    /// <inheritdoc/>
    public async IAsyncEnumerable<AIChatCompletionDelta> StreamSalesResponseAsync(
        SalesChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.SessionId))
            throw new ArgumentException("SessionId must not be empty.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("Message must not be empty.", nameof(request));

        logger.LogInformation("SalesAgent processing message for session {SessionId}", request.SessionId);

        // Retrieve or create server-side history for the session.
        var entry = _sessions.GetOrAdd(request.SessionId, _ =>
        {
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, SystemPrompt)
            };

            // Seed from client-provided history when no server-side session exists yet.
            if (request.History is { Count: > 0 })
            {
                foreach (var msg in request.History)
                {
                    var role = msg.Role switch
                    {
                        AIChatRole.Assistant => ChatRole.Assistant,
                        _ => ChatRole.User
                    };
                    messages.Add(new ChatMessage(role, msg.Content));
                }
            }

            return new SessionEntry(messages, new object());
        });

        // Append the current user message under the per-session lock.
        lock (entry.Lock)
        {
            entry.History.Add(new ChatMessage(ChatRole.User, request.Message));
        }

        var chatOptions = new ChatOptions { Tools = _tools };

        // Take a snapshot of the history to pass to the LLM without holding the lock during I/O.
        List<ChatMessage> historyCopy;
        lock (entry.Lock)
        {
            historyCopy = [.. entry.History];
        }

        // Collect the full response so we can add it to history after streaming.
        var fullResponse = new StringBuilder();

        await foreach (var update in chatClient.GetStreamingResponseAsync(historyCopy, chatOptions, cancellationToken))
        {
            var text = update.Text;
            if (string.IsNullOrEmpty(text)) continue;

            fullResponse.Append(text);
            yield return new AIChatCompletionDelta(Delta: new AIChatMessageDelta
            {
                Role = AIChatRole.Assistant,
                Context = new AIChatAgentInfo("SalesAgent"),
                Content = text
            });
        }

        // Persist the assistant's complete response to the session history.
        if (fullResponse.Length > 0)
        {
            lock (entry.Lock)
            {
                entry.History.Add(new ChatMessage(ChatRole.Assistant, fullResponse.ToString()));
            }
        }
    }

    private static List<AITool> BuildTools(
        IAiSearchService aiSearchService,
        IFlowerService flowerService,
        IOrderService orderService,
        ILogger logger)
    {
        var searchFlowersByOccasion = AIFunctionFactory.Create(
            async ([Description("The occasion or theme to search flowers for, e.g. 'birthday', 'wedding', 'sympathy'")] string occasion) =>
            {
                logger.LogInformation("SalesAgent: SearchFlowersByOccasion({Occasion})", occasion);
                var result = await aiSearchService.Search(occasion);
                return JsonSerializer.Serialize(result, _jsonOptions);
            },
            "SearchFlowersByOccasion",
            "Searches for flowers suitable for the given occasion or theme and returns a list of matching products with pricing.");

        var getFlowerDetails = AIFunctionFactory.Create(
            async ([Description("The integer ID of the flower product to retrieve details for")] int flowerId) =>
            {
                logger.LogInformation("SalesAgent: GetFlowerDetails({FlowerId})", flowerId);
                var result = await flowerService.GetFlowerByIdAsync(flowerId);
                if (result is null)
                    return "Flower not found.";
                return JsonSerializer.Serialize(result, _jsonOptions);
            },
            "GetFlowerDetails",
            "Retrieves detailed information (description, price, stock) about a specific flower product by its ID.");

        var placeOrder = AIFunctionFactory.Create(
            async ([Description("Order details including optional delivery info and the list of flower items to purchase")] CreateOrderDto orderRequest) =>
            {
                logger.LogInformation("SalesAgent: PlaceOrder called");
                var result = await orderService.PlaceOrderAsync(orderRequest);
                if (!result.IsSuccess)
                    return $"Order placement failed: {result.Error}";
                return JsonSerializer.Serialize(result.Value, _jsonOptions);
            },
            "PlaceOrder",
            "Places a flower order on behalf of the customer. Only call this tool after the customer has explicitly confirmed the order details including items and delivery information.");

        return [searchFlowersByOccasion, getFlowerDetails, placeOrder];
    }
}
