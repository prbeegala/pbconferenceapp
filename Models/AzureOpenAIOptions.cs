namespace ConferenceApp.Models
{
    public class AzureOpenAIOptions
    {
        public const string SectionName = "AzureOpenAI";

        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string DeploymentName { get; set; } = string.Empty;
        public int MaxTokens { get; set; } = 500;
        public double Temperature { get; set; } = 0.7;
        public bool Enabled { get; set; } = true;
    }
}