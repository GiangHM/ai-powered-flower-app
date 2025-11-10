using FlowerShop.Application.Dtos;
using FlowerShop.Application.Features.Flowers.Queries;
using FlowerShop.Application.Interfaces;
using FlowerShop.Infrastructure.VectorDb;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;

namespace FlowerShop.Infrastructure.Services
{
    public class AiSearchService : IAiSearchService
    {
        private const string SystemPrompt = "You are a useful assistant. You always reply with a short and funny message." +
            " If you do not know an answer, you say 'I don't know that.' You only answer questions related to flowers products." +
            " For any other type of questions, explain to the user that you only answer flower products questions." +
            " Do not store memory of the chat conversation.";

        private readonly IVectorDbContext _vectorDbContext;
        private readonly IFlowerGetByIds<IEnumerable<FlowerResponseItem>> _flowerQuery;
        private readonly IChatCompletionService _chatCompletionService;
        public AiSearchService(IVectorDbContext vectorDbContext
            , IChatCompletionService chatCompletionService
            , IFlowerGetByIds<IEnumerable<FlowerResponseItem>> flowerQuery)
        {
            _vectorDbContext = vectorDbContext;
            _flowerQuery = flowerQuery;
            _chatCompletionService = chatCompletionService;
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

            // TODO:
            // 1. Break down to another chat completion service
            // 2. Use some prompt techniques to improve the response from AI

            //Construct AiSearchResponse with AI chat completion service
            // let's improve the response message
            var sbFoundProducts = new StringBuilder();
            foreach (var item in flowers)
            {
                sbFoundProducts.AppendLine($"- Product {item.Id}:");
                sbFoundProducts.AppendLine($"  - Name: {item.Name}");
                sbFoundProducts.AppendLine($"  - UnitPrice: {item.UnitPrice}");
            }

            var prompt = @$"You are an intelligent assistant helping clients with their search about flower products. 
            Generate a catchy and friendly message using the information below.
            Add a comparison between the products found and the search criteria.
            Include products details.
                - User Question: {searchString}
                - Found Products: {sbFoundProducts}";

            var chatMessage = new ChatHistory();
            chatMessage.AddSystemMessage(SystemPrompt);
            chatMessage.AddUserMessage(prompt);

            var reply = await _chatCompletionService.GetChatMessageContentAsync(chatMessage);
            response.Response = reply.ToString();

            return response;
        }
    }
}
