using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Google;
using System.Text;

namespace FlowerShop.Infrastructure.Agent.AgentPlugins
{
    public sealed class GoogleTextSearchSettings
    {
        public string GoogleApiKey { get; set; } = "";

        public string GoogleSearchEngineId { get; set; } = string.Empty;
    }
    public sealed class GoogleTextSearchPlugin
    {
        private readonly GoogleTextSearch? _googleTextSearch;
        public GoogleTextSearchPlugin(IOptions<GoogleTextSearchSettings> settings)
        {
            _googleTextSearch = new GoogleTextSearch(
                initializer: new() { ApiKey = settings.Value.GoogleApiKey },
                searchEngineId: settings.Value.GoogleSearchEngineId);
        }

        [KernelFunction]
        public async Task<string> ResearchProduct(string searchText)
        {
            if (searchText == null)
                throw new ArgumentNullException(nameof(searchText));
            if (_googleTextSearch == null)
                return "No research found because of the error";

            KernelSearchResults<string> stringResults = await _googleTextSearch.SearchAsync(searchText, new() { Top = 1, Skip = 0 });

            StringBuilder finalResult = new StringBuilder(); 
            await foreach (string result in stringResults.Results)
            {
                finalResult.Append(result);
            }

            return finalResult.ToString();
        }
    }
}
