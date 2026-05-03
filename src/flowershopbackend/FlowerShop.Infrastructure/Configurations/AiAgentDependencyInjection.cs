using FlowerShop.Infrastructure.Agent.AgentTools;
using FlowerShop.Infrastructure.AIServices;
using FlowerShop.Infrastructure.Options;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;

namespace FlowerShop.Infrastructure.Configurations
{
    public static class AiDependencyInjection
    {
        public static WebApplicationBuilder AddAiAgents(this WebApplicationBuilder builder, IConfiguration config)
        {
            builder.Services.AddOptions<GitHubModelOption>()
                .Bind(config.GetSection("GitHubModel"));

            var options = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<GitHubModelOption>>().Value;

            builder.AddAIAgent("SummaryAgent", (sp, key) =>
            {
                var openAIOptions = new OpenAIClientOptions
                {
                    Endpoint = new Uri(options.Endpoint)
                };

                var chatClient = new ChatClientBuilder(new OpenAIClient(new ApiKeyCredential(options.GithubToken), openAIOptions)
                    .GetChatClient(options.ChatModelId)
                    .AsIChatClient())
                    .UseOpenTelemetry(
                        loggerFactory: null,
                        sourceName: "FlowerShop.AiServices.SummaryAgent",
                        configure: otel =>
                        {
                            otel.EnableSensitiveData = true;
                        })
                    .Build();


                var agent = chatClient.CreateAIAgent(
                    name: "SummaryAgent",
                    instructions: "You are an summary agent helping clients to summarize the flower product results of their searching ." +
                    " You always reply with a short and helpful message." +
                    " You only summarize the flowers products results." +
                    " Do not store memory of the chat conversation.");

                return agent;
            });

            // Register a named HttpClient and the Google search service for the ResearcherAgent.
            builder.Services.AddHttpClient("GoogleSearch");
            builder.Services.AddTransient<GoogleTextSearchService>();

            // ResearcherAgent: an AIAgent that uses the GoogleSearch tool to find information.
            // This mirrors the Semantic Kernel Plugins.AddFromObject() pattern:
            //   GoogleTextSearchPlugin googleSearchPlugin = new(...);
            //   researcherKernel.Plugins.AddFromObject(googleSearchPlugin);
            // Equivalent in MS Agents AI:
            //   var googleSearchFn = AIFunctionFactory.Create(SearchAsync_MethodInfo, instance);
            //   chatClient.CreateAIAgent(..., tools: [googleSearchFn])
            builder.AddAIAgent("ResearcherAgent", (sp, key) =>
            {
                var openAIOptions = new OpenAIClientOptions
                {
                    Endpoint = new Uri(options.Endpoint)
                };

                // Build the Google search tool from the service class method — equivalent to
                // SK's Plugins.AddFromObject(googleSearchPlugin) with [KernelFunction] methods.
                var searchService = sp.GetRequiredService<GoogleTextSearchService>();
                var googleSearchFn = AIFunctionFactory.Create(
                    typeof(GoogleTextSearchService).GetMethod(nameof(GoogleTextSearchService.SearchAsync))!,
                    searchService,
                    name: "GoogleSearch",
                    description: "Searches the web using Google Custom Search API and returns relevant results.");

                var chatClient = new ChatClientBuilder(new OpenAIClient(new ApiKeyCredential(options.GithubToken), openAIOptions)
                    .GetChatClient(options.ChatModelId)
                    .AsIChatClient())
                    .UseOpenTelemetry(
                        loggerFactory: null,
                        sourceName: "FlowerShop.AiServices.ResearcherAgent",
                        configure: otel =>
                        {
                            otel.EnableSensitiveData = true;
                        })
                    .UseFunctionInvocation()
                    .Build();

                return chatClient.CreateAIAgent(
                    name: "ResearcherAgent",
                    description: "Researcher agent that uses Google Search to find information",
                    instructions:
                        "You are an expert researcher. Given a topic, use the GoogleSearch tool to find " +
                        "relevant, accurate information from the web. Provide comprehensive research results " +
                        "including key facts, statistics, and interesting details that a copywriter can use " +
                        "to create an engaging marketing article.",
                    tools: [googleSearchFn]);
            });

            // WriterAgent: generates a marketing article from research results.
            builder.AddAIAgent("WriterAgent", (sp, key) =>
            {
                var openAIOptions = new OpenAIClientOptions
                {
                    Endpoint = new Uri(options.Endpoint)
                };

                var chatClient = new ChatClientBuilder(new OpenAIClient(new ApiKeyCredential(options.GithubToken), openAIOptions)
                    .GetChatClient(options.ChatModelId)
                    .AsIChatClient())
                    .UseOpenTelemetry(
                        loggerFactory: null,
                        sourceName: "FlowerShop.AiServices.WriterAgent",
                        configure: otel =>
                        {
                            otel.EnableSensitiveData = true;
                        })
                    .Build();

                return chatClient.CreateAIAgent(
                    name: "WriterAgent",
                    instructions:
                        "You are an expert copywriter who creates fun and engaging marketing articles. " +
                        "Given research data and a product description, write a compelling article " +
                        "between 800 and 1000 words. Format the article as markdown but do not include ```markdown``` code fences. " +
                        "Do not store memory of the chat conversation.");
            });

            // Sales client: a plain IChatClient with function-invocation enabled for the SalesAgent.
            // Tools (SearchFlowersByOccasion, GetFlowerDetails, PlaceOrder) are registered at
            // call-time via ChatOptions so they can capture request-scoped service instances.
            builder.Services.AddKeyedSingleton<IChatClient>("salesclient", (sp, key) =>
            {
                var openAIOptions = new OpenAIClientOptions
                {
                    Endpoint = new Uri(options.Endpoint)
                };

                return new ChatClientBuilder(
                    new OpenAIClient(new ApiKeyCredential(options.GithubToken), openAIOptions)
                        .GetChatClient(options.ChatModelId)
                        .AsIChatClient())
                    .UseOpenTelemetry(
                        loggerFactory: null,
                        sourceName: "FlowerShop.AiServices.SalesClient",
                        configure: otel => { otel.EnableSensitiveData = true; })
                    .UseFunctionInvocation()
                    .Build();
            });

            // Vision client: a plain IChatClient backed by a GPT-4o (vision-capable) model.
            // Used by FlowerImageService to analyse uploaded flower photos.
            builder.Services.AddKeyedSingleton<IChatClient>("visionclient", (sp, key) =>
            {
                var openAIOptions = new OpenAIClientOptions
                {
                    Endpoint = new Uri(options.Endpoint)
                };

                var modelId = string.IsNullOrWhiteSpace(options.VisionModelId)
                    ? options.ChatModelId
                    : options.VisionModelId;

                return new ChatClientBuilder(
                    new OpenAIClient(new ApiKeyCredential(options.GithubToken), openAIOptions)
                        .GetChatClient(modelId)
                        .AsIChatClient())
                    .UseOpenTelemetry(
                        loggerFactory: null,
                        sourceName: "FlowerShop.AiServices.VisionClient",
                        configure: otel => { otel.EnableSensitiveData = false; })
                    .Build();
            });

            return builder;
        }
    }
}

