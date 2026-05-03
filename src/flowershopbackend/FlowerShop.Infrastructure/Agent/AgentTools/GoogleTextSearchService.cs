using System.ComponentModel;
using Microsoft.Extensions.Configuration;

namespace FlowerShop.Infrastructure.Agent.AgentTools;

/// <summary>
/// Provides Google Custom Search integration for AI agent tool use.
/// This is the MS Agents AI equivalent of the Semantic Kernel GoogleTextSearchPlugin pattern:
/// register the method as an <see cref="Microsoft.Extensions.AI.AIFunction"/> via
/// <c>AIFunctionFactory.Create(SearchAsync_MethodInfo, instance)</c> and pass it as a
/// tool to <c>IChatClient.CreateAIAgent(..., tools: [googleSearchFn])</c>.
/// </summary>
public class GoogleTextSearchService(IHttpClientFactory httpClientFactory, IConfiguration config)
{
    private const int MaxResults = 5;

    /// <summary>
    /// Searches the web using Google Custom Search API and returns raw JSON results.
    /// </summary>
    /// <param name="query">The search query to find information on the web.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>JSON string of search results, or an error message.</returns>
    [Description("Searches the web using Google Custom Search API and returns relevant search results.")]
    public async Task<string> SearchAsync(
        [Description("The search query to find relevant information on the web")] string query,
        CancellationToken cancellationToken = default)
    {
        var apiKey = config["GoogleTextSearchSettings:GoogleApiKey"] ?? string.Empty;
        var engineId = config["GoogleTextSearchSettings:SearchEngineId"] ?? string.Empty;

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(engineId))
            return "Google Search is not configured. Set GoogleTextSearchSettings:GoogleApiKey and GoogleTextSearchSettings:SearchEngineId.";

        try
        {
            var http = httpClientFactory.CreateClient("GoogleSearch");
            var url = $"https://www.googleapis.com/customsearch/v1" +
                      $"?key={apiKey}&cx={engineId}" +
                      $"&q={Uri.EscapeDataString(query)}&num={MaxResults}";
            return await http.GetStringAsync(url, cancellationToken);
        }
        catch (Exception ex)
        {
            return $"Search failed: {ex.Message}";
        }
    }
}
