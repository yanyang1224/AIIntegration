using System.Collections.Generic;
using System.Threading.Tasks;
using AIIntegration.Library.Models;
using System.Runtime.CompilerServices;

namespace AIIntegration.Library.Interfaces
{
    public interface IAIService
    {
        Task<AIResponse> GenerateResponseAsync(AIRequest request);
        IAsyncEnumerable<string> GenerateStreamResponseAsync(AIRequest request);
    }
}