using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.IO;  //
using System.Threading.Tasks;
using AIIntegration.Library.Interfaces;
using AIIntegration.Library.Models;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Threading;
using AIIntegration.Library.Attributes;
using AIIntegration.Library.Enums;
using System.Linq;

namespace AIIntegration.Library.Services.Qwen
{
    [AIService(AIServiceType.Qwen)]
    public class QwenService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceConfig _config;

        public QwenService(AIServiceConfig config)
        {
            _config = config.Qwen;
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
            var qwenRequest = new
            {
                messages = request.Messages.Select(m => new { role = m.Role, content = m.Content }).ToList(),
                max_tokens = request.MaxTokens
            };

            var content = new StringContent(JsonConvert.SerializeObject(qwenRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_config.ApiUrl, content);

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

        /// <summary>
        /// 生成流式AI响应的异步方法
        /// </summary>
        /// <param name="request">AI请求对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>包含响应字符串的异步枚举</returns>
        public async IAsyncEnumerable<string> GenerateStreamResponseAsync(AIRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var qwenRequest = new
            {
                messages = request.Messages.Select(m => new { role = m.Role, content = m.Content }).ToList(),
                max_tokens = request.MaxTokens,
                stream = true
            };

            var content = new StringContent(JsonConvert.SerializeObject(qwenRequest), Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(_config.ApiUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line) || !line.StartsWith("data:")) continue;

                var jsonData = line.Substring(5).Trim(); // Remove "data: " prefix
                if (jsonData == "[DONE]") break;

                var qwenResponse = JsonConvert.DeserializeObject<dynamic>(jsonData);
                if (qwenResponse.choices != null && qwenResponse.choices[0].delta?.content != null)
                {
                    yield return qwenResponse.choices[0].delta.content.ToString();
                }
            }
        }
    }
}