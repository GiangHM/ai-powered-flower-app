using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace FlowerShop.Infrastructure.HttpClientHandlers
{
    /// <summary>
    /// This is the workaround of the below github issue
    /// github issue: https://github.com/ssone95/ChromaDB.Client/issues/69#issuecomment-2918351412
    /// </summary>
    public class ChromaApiV1ToV2DelegatingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var baseAddress = new Uri($"{request.RequestUri.Scheme}://{request.RequestUri.Host}:{request.RequestUri.Port}/api/v2/");

            var queries = HttpUtility.ParseQueryString(request.RequestUri.Query);

            var tenant = queries["tenant"];
            queries.Remove("tenant");

            var database = queries["database"];
            queries.Remove("database");

            if (string.IsNullOrWhiteSpace(tenant) || string.IsNullOrWhiteSpace(database))
            {
                var httpClient = new HttpClient
                {
                    BaseAddress = baseAddress
                };

                var authIdentityResponse = await httpClient.GetFromJsonAsync<AuthIdentityResponse>("auth/identity", cancellationToken);

                tenant ??= authIdentityResponse?.Tenant;
                database ??= authIdentityResponse?.Databases.FirstOrDefault();
            }

            var uriBuilder = new UriBuilder(request.RequestUri)
            {
                Query = queries.ToString()
            };

            request.RequestUri = new Uri(uriBuilder.Uri.ToString().Replace(baseAddress.ToString(), $"{baseAddress}tenants/{tenant}/databases/{database}/"));

            var response = await base.SendAsync(request, cancellationToken);

            if (request.RequestUri.AbsolutePath.EndsWith("/query"))
            {
                var originalContent = await response.Content.ReadAsStringAsync(cancellationToken);

                var payload = JsonSerializer.Deserialize<Dictionary<string, object?>>(originalContent) ?? [];

                payload["data"] = null;

                var modifiedContent = JsonSerializer.Serialize(payload);

                response.Content = new StringContent(modifiedContent, Encoding.UTF8, "application/json");
            }

            return response;
        }
    }
    public class AuthIdentityResponse
    {
        [JsonPropertyName("user_id")]
        public required string UserId { get; set; }

        public required string Tenant { get; set; }

        public required string[] Databases { get; set; }
    }
}
