using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Options;
using ConferenceApp.Models;
using OpenAI.Chat;
using Azure.Identity;

namespace ConferenceApp.Services
{
    public interface IAISuggestionService
    {
        Task<AISuggestionResponse> GetSessionSuggestionsAsync(AISuggestionRequest request);
        bool IsEnabled { get; }
    }

    public class AISuggestionService : IAISuggestionService
    {
        private readonly AzureOpenAIClient? _client;
        private readonly AzureOpenAIOptions _options;
        private readonly ILogger<AISuggestionService> _logger;

        public bool IsEnabled => _options.Enabled && _client != null && 
                                !string.IsNullOrEmpty(_options.Endpoint) && 
                                !string.IsNullOrEmpty(_options.DeploymentName);

        public AISuggestionService(IOptions<AzureOpenAIOptions> options, ILogger<AISuggestionService> logger)
        {
            _options = options.Value;
            _logger = logger;

            // Check if configuration is available before initializing client
            if (_options.Enabled && 
                !string.IsNullOrEmpty(_options.Endpoint) && 
                !string.IsNullOrEmpty(_options.DeploymentName))
            {
                try
                {
                    _logger.LogInformation("Initializing Azure OpenAI client with endpoint: {Endpoint}", _options.Endpoint);
                    
                    // Use DefaultAzureCredential for authentication (more secure than API keys)
                    var credential = new DefaultAzureCredential();
                    _client = new AzureOpenAIClient(new Uri(_options.Endpoint), credential);
                    
                    _logger.LogInformation("Azure OpenAI client initialized successfully with DefaultAzureCredential");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize Azure OpenAI client with DefaultAzureCredential");
                    
                    // Fallback to API key if DefaultAzureCredential fails and API key is provided
                    if (!string.IsNullOrEmpty(_options.ApiKey))
                    {
                        try
                        {
                            _logger.LogInformation("Attempting fallback to API key authentication");
                            _client = new AzureOpenAIClient(
                                new Uri(_options.Endpoint),
                                new AzureKeyCredential(_options.ApiKey));
                            _logger.LogInformation("Azure OpenAI client initialized successfully with API key fallback");
                        }
                        catch (Exception fallbackEx)
                        {
                            _logger.LogError(fallbackEx, "Failed to initialize Azure OpenAI client with API key fallback");
                            _client = null;
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No API key provided for fallback authentication");
                        _client = null;
                    }
                }
            }
            else
            {
                _logger.LogWarning("Azure OpenAI service not configured. Enabled: {Enabled}, Endpoint: {HasEndpoint}, DeploymentName: {HasDeploymentName}",
                    _options.Enabled,
                    !string.IsNullOrEmpty(_options.Endpoint),
                    !string.IsNullOrEmpty(_options.DeploymentName));
            }
        }

        public async Task<AISuggestionResponse> GetSessionSuggestionsAsync(AISuggestionRequest request)
        {
            if (!IsEnabled || _client == null)
            {
                return new AISuggestionResponse
                {
                    Success = false,
                    ErrorMessage = "AI suggestion service is not available. Please check your configuration.",
                    SuggestedTitle = request.CurrentTitle ?? "",
                    SuggestedDescription = request.CurrentDescription ?? ""
                };
            }

            try
            {
                var chatClient = _client.GetChatClient(_options.DeploymentName);
                var systemPrompt = "You are an expert conference session curator. Improve session titles and descriptions while keeping the speaker's core ideas intact. Make titles engaging and descriptions compelling. Return JSON format: {\"title\": \"improved title\", \"description\": \"enhanced description\"}";
                var userPrompt = $"Title: {request.CurrentTitle}, Description: {request.CurrentDescription}, Technology: {request.Technology}";

                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(systemPrompt),
                    new UserChatMessage(userPrompt)
                };

                var options = new ChatCompletionOptions
                {
                    MaxOutputTokenCount = _options.MaxTokens,
                    Temperature = (float)_options.Temperature
                };

                var response = await chatClient.CompleteChatAsync(messages, options);
                var content = response.Value.Content[0].Text;

                return new AISuggestionResponse
                {
                    Success = true,
                    SuggestedTitle = request.CurrentTitle ?? "",
                    SuggestedDescription = request.CurrentDescription ?? ""
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Azure OpenAI service");
                return new AISuggestionResponse
                {
                    Success = false,
                    ErrorMessage = "Unable to generate suggestions at this time. Please try again later.",
                    SuggestedTitle = request.CurrentTitle ?? "",
                    SuggestedDescription = request.CurrentDescription ?? ""
                };
            }
        }
    }

    public class AISuggestionRequest
    {
        public string? CurrentTitle { get; set; }
        public string? CurrentDescription { get; set; }
        public string? Technology { get; set; }
        public int Duration { get; set; }
        public string? Level { get; set; }
        public string? Format { get; set; }
    }

    public class AISuggestionResponse
    {
        public bool Success { get; set; }
        public string SuggestedTitle { get; set; } = string.Empty;
        public string SuggestedDescription { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}