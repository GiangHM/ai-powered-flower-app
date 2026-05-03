using FlowerShop.Application.Dtos;
using FlowerShop.Application.Features.Flowers.Queries;
using FlowerShop.Application.Interfaces;
using FlowerShop.Infrastructure.VectorDb;
using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace FlowerShop.Infrastructure.Services
{
    public class AiSearchService : IAiSearchService
    {
        private readonly IVectorDbContext _vectorDbContext;
        private readonly IFlowerGetByIds<IEnumerable<FlowerResponseItem>> _flowerQuery;
        private readonly AIAgent _summaryAgent;
        public AiSearchService(IVectorDbContext vectorDbContext
            , [FromKeyedServices("SummaryAgent")] AIAgent summaryAgent
            , IFlowerGetByIds<IEnumerable<FlowerResponseItem>> flowerQuery)
        {
            _vectorDbContext = vectorDbContext;
            _flowerQuery = flowerQuery;
            _summaryAgent = summaryAgent;
        }
        /// <summary>
        /// Implements a semantic search using AI to find relevant flowers based on the search string.
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<AiSearchResponse> Search(string searchString)
        {
            var response = new AiSearchResponse
            {
                Response = $"I don't know the answer for your question. Your question is: [{searchString}]"
            };

            //Call Semantic search
            var foundIds = await _vectorDbContext.SemanticSearchAsync(searchString);

            if (!foundIds.Any())
            {
                return response;
            }
            //Retrieve relevant flower data
            var flowers = await _flowerQuery.Handle(foundIds);
            response.Flowers = flowers.ToList();

            //Construct AiSearchResponse with AI chat completion service
            // let's improve the response message
            var sbFoundProducts = new StringBuilder();
            foreach (var item in flowers)
            {
                sbFoundProducts.AppendLine($"Product {item.Id}:");
                sbFoundProducts.AppendLine($"  + Name: {item.Name}");
                sbFoundProducts.AppendLine($"  + UnitPrice: {item.UnitPrice}");
            }

            var prompt = @$"Generate a catchy and friendly message using the information below.
            Add a comparison between the products found and the search criteria.
            Include products details.
                - User Question: {searchString}
                - Found Products:
                    {sbFoundProducts}";

            var reply = new StringBuilder();
            await foreach (var update in _summaryAgent.RunStreamingAsync(prompt))
            {
                reply.Append(update);
                reply.AppendLine();
            }

            response.Response = reply.ToString();

            return response;
        }
    }
}
