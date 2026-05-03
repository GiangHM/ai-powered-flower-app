namespace FlowerShop.Infrastructure.Options
{
    public class GitHubModelOption
    {
        public string ChatModelId { get; set; } = string.Empty;

        /// <summary>
        /// Model ID for the vision (image-analysis) chat client.
        /// Must reference a multimodal model such as "gpt-4o".
        /// Falls back to <see cref="ChatModelId"/> when not set, but note that not all
        /// chat models support image input — ensure the fallback model is vision-capable.
        /// </summary>
        public string VisionModelId { get; set; } = string.Empty;

        public string EmbeddingModel { get; set; } = string.Empty;
        public string GithubToken { get; set; } = string.Empty;
        public string Endpoint { get; set;} = string.Empty;
    }
}
