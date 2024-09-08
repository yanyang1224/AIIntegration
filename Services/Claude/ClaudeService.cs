using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AIIntegration.Interfaces;
using AIIntegration.Models;
using Newtonsoft.Json;

namespace AIIntegration.Services.Claude
{
    public class ClaudeService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string ApiUrl = "https://api.anthropic.com/v1/complete";

        public ClaudeService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        }

        public async Task<AIResponse> GenerateResponseAsync(AIRequest request)
        {
            var claudeRequest = new
            {
                prompt = request.Prompt,
                max_tokens_to_sample = request.MaxTokens,
                model = "claude-v1"
            };

            var content = new StringContent(JsonConvert.SerializeObject(claudeRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var claudeResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                return new AIResponse
                {
                    GeneratedText = claudeResponse.completion,
                    TokensUsed = claudeResponse.stop_reason == "max_tokens" ? request.MaxTokens : 0
                };
            }

            throw new Exception($"Claude API request failed with status code: {response.StatusCode}");
        }
    }
}