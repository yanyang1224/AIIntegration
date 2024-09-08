using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AIIntegration.Interfaces;
using AIIntegration.Models;
using Newtonsoft.Json;

namespace AIIntegration.Services.DeepSpeek
{
    public class DeepSpeekService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string ApiUrl = "https://api.deepspeek.com/v1/generate"; // 请替换为实际的 API 地址

        public DeepSpeekService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<AIResponse> GenerateResponseAsync(AIRequest request)
        {
            var deepSpeekRequest = new
            {
                text = request.Prompt,
                max_length = request.MaxTokens
            };

            var content = new StringContent(JsonConvert.SerializeObject(deepSpeekRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var deepSpeekResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                return new AIResponse
                {
                    GeneratedText = deepSpeekResponse.generated_text,
                    TokensUsed = deepSpeekResponse.tokens_used ?? 0
                };
            }

            throw new Exception($"DeepSpeek API request failed with status code: {response.StatusCode}");
        }
    }
}