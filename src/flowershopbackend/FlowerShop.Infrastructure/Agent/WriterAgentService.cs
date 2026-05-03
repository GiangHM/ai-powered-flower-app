using FlowerShop.Application.Dtos.ChatFeature;
using FlowerShop.Application.Interfaces;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlowerShop.Infrastructure.Agent;

/// <summary>
/// Orchestrates the ResearcherAgent (AIAgent with Google Search tool + function invocation)
/// and WriterAgent (AIAgent) to produce a streaming marketing article.
/// </summary>
public class WriterAgentService(
    [FromKeyedServices("ResearcherAgent")] AIAgent researcherAgent,
    [FromKeyedServices("WriterAgent")] AIAgent writerAgent,
    ILogger<WriterAgentService> logger) : IWriterAgentService
{
    private const string WriterPromptTemplate =
        """
        Research Topic: {0}

        Research Results:
        {1}

        Writing Instructions: {2}

        Based on the research above, write a fun and engaging marketing article
        following the writing instructions. Format the output as markdown but do not
        include ```markdown``` code fences.
        """;

    /// <inheritdoc/>
    public async IAsyncEnumerable<AIChatCompletionDelta> ProcessStreamingAsync(
        CreateWriterRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Phase 1: Research — ResearcherAgent uses Google Search tool automatically via
        // UseFunctionInvocation() middleware and the tool registered in CreateAIAgent.
        logger.LogInformation("ResearcherAgent starting for: {Research}", request.Research);
        var researchResult = new StringBuilder();

        await foreach (var update in researcherAgent.RunStreamingAsync(request.Research, cancellationToken: cancellationToken))
        {
            if (string.IsNullOrEmpty(update.Text)) continue;

            researchResult.Append(update.Text);
            yield return new AIChatCompletionDelta(Delta: new AIChatMessageDelta
            {
                Role = AIChatRole.Assistant,
                Context = new AIChatAgentInfo("Researcher"),
                Content = update.Text
            });
        }

        // Phase 2: Write — WriterAgent receives research results and writing instructions.
        logger.LogInformation("WriterAgent starting.");
        var writerPrompt = string.Format(WriterPromptTemplate,
            request.Research,
            researchResult,
            request.Writing);

        await foreach (var update in writerAgent.RunStreamingAsync(writerPrompt, cancellationToken: cancellationToken))
        {
            if (string.IsNullOrEmpty(update.Text)) continue;

            yield return new AIChatCompletionDelta(Delta: new AIChatMessageDelta
            {
                Role = AIChatRole.Assistant,
                Context = new AIChatAgentInfo("Writer"),
                Content = update.Text
            });
        }
    }
}
