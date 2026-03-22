namespace Backend.API.Configuration
{
    public sealed class OpenAiOptions
    {
        public const string SectionName = "OpenAI";

        public string ApiKey { get; set; } = string.Empty;

        public string ChatModel { get; set; } = string.Empty;

        public string EmbeddingModel { get; set; } = string.Empty;
    }
}
