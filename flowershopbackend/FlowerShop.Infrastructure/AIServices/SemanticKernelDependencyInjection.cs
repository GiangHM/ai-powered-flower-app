using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;

namespace FlowerShop.Infrastructure.AIServices
{
    public static class SemanticKernelDependencyInjection
    {
        public static IServiceCollection AddKernelChatCompletionServices(this IServiceCollection services
            , IConfiguration config)
        {

            services.AddOptions<GitHubModelOption>()
                .Bind(config.GetSection("GitHubModel"));

            var options = services.BuildServiceProvider().GetRequiredService<IOptions<GitHubModelOption>>().Value;

            // Register Chat Client related services here
            var githubToken = options.GithubToken ?? Environment.GetEnvironmentVariable("GitHubToken");

            services.AddKernel()
                .AddOpenAIChatCompletion(
                    modelId: options.ChatModelId,
                    new OpenAIClient(new ApiKeyCredential(githubToken), new OpenAIClientOptions { Endpoint = new Uri(options.Endpoint) })
                )
                .ConfigureOpenTelemetry(config);

           // Finally, create the Kernel service with the service provider and plugin collection
            //services.AddTransient((serviceProvider) =>
            //{
            //    return new Kernel(serviceProvider);
            //});

            return services;
        }

        public static IServiceCollection AddKernelEmbedding(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<GitHubModelOption>()
                .Bind(config.GetSection("GitHubModel"));

            var options = services.BuildServiceProvider().GetRequiredService<IOptions<GitHubModelOption>>().Value;

            var githubToken = options.GithubToken ?? Environment.GetEnvironmentVariable("GitHubToken");

//#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
//            services.AddOpenAIEmbeddingGenerator(options.EmbeddingModel
//                , new OpenAIClient(new ApiKeyCredential(githubToken), new OpenAIClientOptions { Endpoint = new Uri(options.Endpoint) })
//                , 1536
//                );
//#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            services.AddKernel()
                .AddOpenAIEmbeddingGenerator(options.EmbeddingModel
                    , new OpenAIClient(new ApiKeyCredential(githubToken)
                        , new OpenAIClientOptions { Endpoint = new Uri(options.Endpoint)}))
                .ConfigureOpenTelemetry (config);

            // Finally, create the Kernel service with the service provider and plugin collection
            //services.AddTransient((serviceProvider) =>
            //{
            //    return new Kernel(serviceProvider);
            //});

            return services;
        }
    }
}
