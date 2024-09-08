using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AIIntegration.Interfaces;
using AIIntegration.Models;
using Newtonsoft.Json;

namespace AIIntegration.Services.Qwen
{
    public class QwenService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string ApiUrl = "https://api.qwen.ai/v1/chat/completions"; // 请替换为实际的 API 地址

        public QwenService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<AIResponse> GenerateResponseAsync(AIRequest request)
        {
            var qwenRequest = new
            {
                messages = new[]
                {
                    new { role = "user", content = request.Prompt }
                },
                max_tokens = request.MaxTokens
            };

            var content = new StringContent(JsonConvert.SerializeObject(qwenRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var qwenResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                return new AIResponse
                {
                    GeneratedText = qwenResponse.choices[0].message.content,
                    TokensUsed = qwenResponse.usage.total_tokens
                };
            }

            throw new Exception($"Qwen API request failed with status code: {response.StatusCode}");
        }
    }
}