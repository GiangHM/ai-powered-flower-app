using FlowerShop.Application.Dtos.ChatFeature;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using System.Text;

namespace FlowerShop.Infrastructure.Agent
{
    public class WriterService(ChatCompletionAgent researcherAgent, ChatCompletionAgent writterAgent)
    {
        public async IAsyncEnumerable<AIChatCompletionDelta> ProcessStreamingRequest(CreateWriterRequest createWriterRequest)
        {
            ChatHistoryAgentThread thread = new();

            StringBuilder researchResult = new StringBuilder();
            KernelArguments arguments = new()
            {
                { "research_context", createWriterRequest.Research }
            };
            await foreach (ChatMessageContent response in researcherAgent.InvokeAsync(thread , options: new() { KernelArguments = arguments }))
            {
                researchResult.AppendLine(response.Content);
                yield return new AIChatCompletionDelta(Delta: new AIChatMessageDelta
                {
                    Role = AIChatRole.Assistant,
                    Context = new AIChatAgentInfo("Researcher"),
                    Content = response.Content,
                });
            }

            writterAgent.Arguments["research_context"] = createWriterRequest.Research;
            writterAgent.Arguments["research_result"] = researchResult.ToString();
            writterAgent.Arguments["assignment"] = createWriterRequest.Writing;
            await foreach (ChatMessageContent response in writterAgent.InvokeAsync())
            {
                yield return new AIChatCompletionDelta(Delta: new AIChatMessageDelta
                {
                    Role = AIChatRole.Assistant,
                    Context = new AIChatAgentInfo(response.AuthorName ?? ""),
                    Content = response.Content,
                });
            }
        }
    }
}
