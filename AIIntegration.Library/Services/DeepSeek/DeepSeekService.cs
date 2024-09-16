using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using AIIntegration.Library.Interfaces;
using AIIntegration.Library.Models;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Threading;
using AIIntegration.Library.Attributes;
using AIIntegration.Library.Enums;

namespace AIIntegration.Library.Services.DeepSeek
{
    [AIService(AIServiceType.DeepSeek)]
    public class DeepSeekService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceConfig _config;

        public DeepSeekService(AIServiceConfig config)
        {
            _config = config.DeepSeek;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
        }

        /// <summary>
        /// 生成AI响应的异步方法
        /// </summary>
        /// <param name="request">AI请求对象</param>
        /// <returns>包含AI响应的任务</returns>
        public async Task<AIResponse> GenerateResponseAsync(AIRequest request)
        {
            var deepSeekRequest = new
            {
                prompt = request.Prompt,
                max_tokens = request.MaxTokens,
                model = _config.Model
            };

            var content = new StringContent(JsonConvert.SerializeObject(deepSeekRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_config.ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var deepSeekResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                return new AIResponse
                {
                    GeneratedText = deepSeekResponse.choices[0].text,
                    TokensUsed = deepSeekResponse.usage.total_tokens
                };
            }

            throw new Exception($"DeepSeek API request failed with status code: {response.StatusCode}");
        }

        /// <summary>
        /// 生成流式AI响应的异步方法
        /// </summary>
        /// <param name="request">AI请求对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>包含响应字符串的异步枚举</returns>
        public async IAsyncEnumerable<string> GenerateStreamResponseAsync(AIRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var deepSeekRequest = new
            {
                prompt = request.Prompt,
                max_tokens = request.MaxTokens,
                model = _config.Model,
                stream = true
            };

            var content = new StringContent(JsonConvert.SerializeObject(deepSeekRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_config.ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line)) continue;

                    var deepSeekResponse = JsonConvert.DeserializeObject<dynamic>(line);
                    if (deepSeekResponse.choices != null && deepSeekResponse.choices[0].text != null)
                    {
                        yield return deepSeekResponse.choices[0].text.ToString();
                    }
                }
            }
            else
            {
                throw new Exception($"DeepSeek API request failed with status code: {response.StatusCode}");
            }
        }
    }
}