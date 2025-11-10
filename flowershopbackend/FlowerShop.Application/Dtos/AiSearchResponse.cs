using System.Text.Json.Serialization;

namespace FlowerShop.Application.Dtos
{
    public class AiSearchResponse
    {
        public string? Response { get; set; }

        [JsonPropertyName("flowers")]
        public List<FlowerResponseItem>? Flowers { get; set; }
    }
}
