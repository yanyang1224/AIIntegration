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

namespace AIIntegration.Library.Services.Claude
{
    [AIService(AIServiceType.Claude)]
    public class ClaudeService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceConfig _config;

        public ClaudeService(AIServiceConfig config)
        {
            _config = config.Claude;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _config.ApiKey);
        }

        /// <summary>
        /// 生成AI响应的异步方法
        /// </summary>
        /// <param name="request">AI请求对象</param>
        /// <returns>包含AI响应的任务</returns>
        public async Task<AIResponse> GenerateResponseAsync(AIRequest request)
        {
            var claudeRequest = new
            {
                prompt = string.Join("\n", request.Messages.Select(m => $"{m.Role}: {m.Content}")),
                max_tokens_to_sample = request.MaxTokens,
                model = _config.Model
            };

            var content = new StringContent(JsonConvert.SerializeObject(claudeRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_config.ApiUrl, content);

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

        /// <summary>
        /// 生成流式AI响应的异步方法
        /// </summary>
        /// <param name="request">AI请求对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>包含响应字符串的异步枚举</returns>
        public async IAsyncEnumerable<string> GenerateStreamResponseAsync(AIRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var claudeRequest = new
            {
                prompt = string.Join("\n", request.Messages.Select(m => $"{m.Role}: {m.Content}")),
                max_tokens_to_sample = request.MaxTokens,
                model = _config.Model,
                stream = true
            };

            var content = new StringContent(JsonConvert.SerializeObject(claudeRequest), Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(_config.ApiUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line)) continue;

                var claudeResponse = JsonConvert.DeserializeObject<dynamic>(line);
                if (claudeResponse.completion != null)
                {
                    yield return claudeResponse.completion.ToString();
                }
            }
        }
    }
}