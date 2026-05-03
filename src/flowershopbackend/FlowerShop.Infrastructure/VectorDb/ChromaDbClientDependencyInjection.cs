using ChromaDB.Client;
using FlowerShop.Infrastructure.HttpClientHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlowerShop.Infrastructure.VectorDb
{
    public static class ChromaDbClientDependencyInjection
    {
        public static IServiceCollection AddChromaDb(this IServiceCollection services, IConfiguration config) 
        {
            services.AddSingleton<ChromaCollectionClient>(serviceProvider =>
            {
                ChromaCollectionClient chromaCollectionClient = null;
                
                // get chromaDB service Uri from configuration
                var chromaDbService = config.GetSection("services:chroma:chromaendpoint:0");
                var chromaDbUri = chromaDbService.Value;

                if (!string.IsNullOrEmpty(chromaDbUri) && !chromaDbUri.EndsWith("/api/v2/"))
                {
                    chromaDbUri += "/api/v2/";
                }

                var handler = new ChromaApiV1ToV2DelegatingHandler
                {
                    InnerHandler = new HttpClientHandler()
                };

                var configOptions = new ChromaConfigurationOptions(uri: chromaDbUri);

                var httpChromaClient = new HttpClient(handler);

                var chromaClient = new ChromaClient(configOptions, httpChromaClient);

                var collection = chromaClient.GetOrCreateCollection("flowers").GetAwaiter().GetResult();
                chromaCollectionClient = new ChromaCollectionClient(collection, configOptions, httpChromaClient);

                return chromaCollectionClient;
            });
            return services;
        }
    }
}
