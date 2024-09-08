using System.Collections.Generic;
using System.Threading.Tasks;
using AIIntegration.Library.Models;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AIIntegration.Library.Interfaces
{
    public interface IAIService
    {
        /// <summary>
        /// 生成AI响应的异步方法
        /// </summary>
        /// <param name="request">AI请求对象</param>
        /// <returns>包含AI响应的任务</returns>
        Task<AIResponse> GenerateResponseAsync(AIRequest request);

        /// <summary>
        /// 生成流式AI响应的异步方法
        /// </summary>
        /// <param name="request">AI请求对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>包含响应字符串的异步枚举</returns>
        IAsyncEnumerable<string> GenerateStreamResponseAsync(AIRequest request, CancellationToken cancellationToken);
    }
}