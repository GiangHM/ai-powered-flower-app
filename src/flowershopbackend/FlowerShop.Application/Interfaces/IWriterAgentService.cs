using FlowerShop.Application.Dtos.ChatFeature;

namespace FlowerShop.Application.Interfaces;

/// <summary>
/// Orchestrates the researcher and writer agents to produce a streamed marketing article.
/// </summary>
public interface IWriterAgentService
{
    /// <summary>
    /// Processes the writer request by first researching the topic and then writing an article,
    /// streaming completion deltas from both agents in sequence.
    /// </summary>
    /// <param name="request">The research and writing prompts for the article.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async stream of completion deltas from researcher and writer agents.</returns>
    IAsyncEnumerable<AIChatCompletionDelta> ProcessStreamingAsync(
        CreateWriterRequest request,
        CancellationToken cancellationToken = default);
}
