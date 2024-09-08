using Microsoft.AspNetCore.Mvc;
using AIIntegration.Library.Interfaces;
using AIIntegration.Library.Models;
using AIIntegration.Library.Services.OpenAI;
using AIIntegration.Library.Services.Claude;
using AIIntegration.Library.Services.DeepSpeek;
using AIIntegration.Library.Services.Qwen;

namespace AIIntegration.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AIController : ControllerBase
{
    private readonly IEnumerable<IAIService> _aiServices;

    public AIController(IEnumerable<IAIService> aiServices)
    {
        _aiServices = aiServices;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<AIResponse>> Generate([FromBody] AIRequest request, [FromQuery] string service)
    {
        var selectedService = GetSelectedService(service);
        if (selectedService == null)
        {
            return NotFound($"Service '{service}' not found.");
        }

        var response = await selectedService.GenerateResponseAsync(request);
        return Ok(response);
    }

    [HttpPost("generate-stream")]
    public IActionResult GenerateStream([FromBody] AIRequest request, [FromQuery] string service)
    {
        var selectedService = GetSelectedService(service);
        if (selectedService == null)
        {
            return NotFound($"Service '{service}' not found.");
        }

        return Ok(selectedService.GenerateStreamResponseAsync(request, HttpContext.RequestAborted));
    }

    private IAIService? GetSelectedService(string service)
    {
        return service.ToLower() switch
        {
            "openai" => _aiServices.OfType<OpenAIService>().FirstOrDefault(),
            "claude" => _aiServices.OfType<ClaudeService>().FirstOrDefault(),
            "deepspeek" => _aiServices.OfType<DeepSpeekService>().FirstOrDefault(),
            "qwen" => _aiServices.OfType<QwenService>().FirstOrDefault(),
            _ => _aiServices.OfType<OpenAIService>().FirstOrDefault()
        };
    }
}