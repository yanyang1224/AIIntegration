using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;  // 添加这行
using AIIntegration.Library.Interfaces;
using AIIntegration.Library.Models;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AIIntegration.Library.Services.OpenAI
{
    public class OpenAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceConfig _config;

        public OpenAIService(AIServiceConfig config)
        {
            _config = config.OpenAI;
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
            var openAIRequest = new
            {
                model = _config.Model,
                prompt = request.Prompt,
                max_tokens = request.MaxTokens
            };

            var content = new StringContent(JsonConvert.SerializeObject(openAIRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_config.ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var openAIResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                return new AIResponse
                {
                    GeneratedText = openAIResponse.choices[0].text,
                    TokensUsed = openAIResponse.usage.total_tokens
                };
            }

            throw new Exception($"OpenAI API request failed with status code: {response.StatusCode}");
        }

        /// <summary>
        /// 生成流式AI响应的异步方法
        /// </summary>
        /// <param name="request">AI请求对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>包含响应字符串的异步枚举</returns>
        public async IAsyncEnumerable<string> GenerateStreamResponseAsync(AIRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var openAIRequest = new
            {
                model = _config.Model,
                prompt = request.Prompt,
                max_tokens = request.MaxTokens,
                stream = true
            };

            var content = new StringContent(JsonConvert.SerializeObject(openAIRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_config.ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (line.StartsWith("data: "))
                    {
                        var data = line.Substring(6);
                        if (data == "[DONE]") break;

                        var result = JsonConvert.DeserializeObject<dynamic>(data);
                        if (result.choices[0].text != null)
                        {
                            yield return result.choices[0].text.ToString();
                        }
                    }
                }
            }
            else
            {
                throw new Exception($"OpenAI API request failed with status code: {response.StatusCode}");
            }
        }
    }
}