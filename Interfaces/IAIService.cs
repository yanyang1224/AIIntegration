using System.Threading.Tasks;
using AIIntegration.Models;

namespace AIIntegration.Interfaces
{
    public interface IAIService
    {
        Task<AIResponse> GenerateResponseAsync(AIRequest request);
        IAsyncEnumerable<string> GenerateStreamResponseAsync(AIRequest request);
    }
}