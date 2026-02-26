using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ConferenceApp.Services;

namespace ConferenceApp.Controllers
{
    [ApiController]
    [Route("api/ai-suggest")]
    [Authorize]
    public class AiSuggestController : ControllerBase
    {
        private readonly IAISuggestionService _aiSuggestionService;
        private readonly ILogger<AiSuggestController> _logger;

        public AiSuggestController(IAISuggestionService aiSuggestionService, ILogger<AiSuggestController> logger)
        {
            _aiSuggestionService = aiSuggestionService;
            _logger = logger;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<AISuggestionResponse>> GetSuggestions([FromBody] AISuggestionRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new AISuggestionResponse
                    {
                        Success = false,
                        ErrorMessage = "Invalid request data.",
                        SuggestedTitle = "",
                        SuggestedDescription = ""
                    });
                }

                if (!_aiSuggestionService.IsEnabled)
                {
                    return Ok(new AISuggestionResponse
                    {
                        Success = false,
                        ErrorMessage = "AI suggestions are currently unavailable.",
                        SuggestedTitle = request.CurrentTitle ?? "",
                        SuggestedDescription = request.CurrentDescription ?? ""
                    });
                }

                var result = await _aiSuggestionService.GetSessionSuggestionsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AI suggestion request");
                return StatusCode(500, new AISuggestionResponse
                {
                    Success = false,
                    ErrorMessage = "An error occurred while generating suggestions.",
                    SuggestedTitle = request?.CurrentTitle ?? "",
                    SuggestedDescription = request?.CurrentDescription ?? ""
                });
            }
        }

        [HttpGet("status")]
        [AllowAnonymous]
        public ActionResult<object> GetStatus()
        {
            return Ok(new
            {
                enabled = _aiSuggestionService.IsEnabled,
                message = _aiSuggestionService.IsEnabled 
                    ? "AI suggestions are available" 
                    : "AI suggestions are currently disabled or not configured"
            });
        }
    }
}