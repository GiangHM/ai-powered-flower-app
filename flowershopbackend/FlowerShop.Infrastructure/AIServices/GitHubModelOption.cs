namespace FlowerShop.Infrastructure.AIServices
{
    public class GitHubModelOption
    {
        public string ChatModelId { get; set; } = string.Empty;
        public string EmbeddingModel { get; set; } = string.Empty;
        public string GithubToken { get; set; } = string.Empty;
        public string Endpoint { get; set;} = string.Empty;
    }
}
