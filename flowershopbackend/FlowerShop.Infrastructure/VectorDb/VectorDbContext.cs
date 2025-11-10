using ChromaDB.Client;
using FlowerShop.Application.Dtos;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Infrastructure.VectorDb
{
    public interface IVectorDbContext
    {
        Task InitMemoryContextAsync(IEnumerable<FlowerVectorDataRequestDto> flowers);
        Task<IEnumerable<long>> SemanticSearchAsync(string searchText, int maxResults = 3);
        Task<bool> DeleteAsync(List<string> ids);
    }
    public class VectorDbContext : IVectorDbContext
    {
        private ILogger<VectorDbContext> _logger;
        private ChromaCollectionClient _collectionClient;
        private IEmbeddingGenerator<string, Embedding<float>> _generator;
        public VectorDbContext(IEmbeddingGenerator<string, Embedding<float>> generator
            , ChromaCollectionClient collectionClient
            , ILogger<VectorDbContext> logger)
        {
            _generator = generator;
            _collectionClient = collectionClient;
            _logger = logger;
        }

        public async Task InitMemoryContextAsync(IEnumerable<FlowerVectorDataRequestDto> flowers)
        {
            var flowerIds = new List<string>();
            var flowerDescriptionEmbeddings = new List<ReadOnlyMemory<float>>();
            var flowerMetadatas = new List<Dictionary<string, object>>();
            
            if (flowers == null || !flowers.Any())
                return;

            foreach (var flower in flowers)
            {
                try
                {

                    flowerIds.Add(flower.Id.ToString());
                    var flowerInfo = $"{flower.Name} priced at {flower.UnitPrice} {flower.UnitCurrency} and is described as {flower.Description}";
                    ReadOnlyMemory<float> embedding = await _generator.GenerateVectorAsync(flowerInfo);
                    flowerDescriptionEmbeddings.Add(embedding);

                    var metadata = new Dictionary<string, object>()
                    {
                        { "Name", flower.Name },
                        { "UnitPrice", flower.UnitPrice },
                        { "UnitCurrency", flower.UnitCurrency }
                    };
                    flowerMetadatas.Add(metadata);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating embedding");
                }
            }

            await _collectionClient.Upsert(flowerIds, flowerDescriptionEmbeddings, flowerMetadatas);

            _logger.LogInformation("Vector DB initialized with flower data");
        }

        public async Task<IEnumerable<long>> SemanticSearchAsync(string searchText, int maxResults = 5)
        {
            _logger.LogInformation("Semantic search string: {0}", searchText);
            ReadOnlyMemory<float> searchEmbedding = await _generator.GenerateVectorAsync(searchText);
            var results = await _collectionClient.Query(
                queryEmbeddings:  searchEmbedding,
                nResults: maxResults,
                include: ChromaQueryInclude.Metadatas | ChromaQueryInclude.Distances);

            var semanticFounds = new List<long>();
            foreach (var item in results)
            {
                if (item.Distance > 0.3)
                    semanticFounds.Add(long.Parse(item.Id));
            }
            return semanticFounds;
        }

        public async Task<bool> DeleteAsync(List<string> ids)
        {
            _logger.LogInformation("You are deleting these ids: {0}", string.Join(",", ids));
            await _collectionClient.Delete(ids);
            return true;
        }
    }
}
