using FlowerShop.Infrastructure.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;
using System.ClientModel;

namespace FlowerShop.Infrastructure.AIServices;

/// <summary>
/// Registers the <see cref="IEmbeddingGenerator{TInput,TEmbedding}"/> using
/// Microsoft.Extensions.AI and the OpenAI SDK — no Semantic Kernel dependency.
/// </summary>
public static class AiEmbeddingDependencyInjection
{
    /// <summary>
    /// Registers <see cref="IEmbeddingGenerator{String, Embedding{Single}}"/> as a singleton,
    /// backed by the GitHub Models OpenAI-compatible endpoint configured in the
    /// <c>GitHubModel</c> section of <paramref name="config"/>.
    /// </summary>
    public static IServiceCollection AddEmbeddingGenerator(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<GitHubModelOption>()
            .Bind(config.GetSection("GitHubModel"));

        var options = services.BuildServiceProvider().GetRequiredService<IOptions<GitHubModelOption>>().Value;

        var githubToken = options.GithubToken ?? Environment.GetEnvironmentVariable("GitHubToken")
            ?? throw new InvalidOperationException("GitHub token is not configured. Set GitHubModel:GithubToken or the GitHubToken environment variable.");

        services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
        {
            var openAIOptions = new OpenAIClientOptions { Endpoint = new Uri(options.Endpoint) };

            return new EmbeddingGeneratorBuilder<string, Embedding<float>>(
                    new OpenAIClient(new ApiKeyCredential(githubToken), openAIOptions)
                        .GetEmbeddingClient(options.EmbeddingModel)
                        .AsIEmbeddingGenerator())
                .UseOpenTelemetry(
                    loggerFactory: null,
                    sourceName: "FlowerShop.AiServices.EmbeddingGenerator",
                    configure: otel => { otel.EnableSensitiveData = false; })
                .Build();
        });

        return services;
    }
}
