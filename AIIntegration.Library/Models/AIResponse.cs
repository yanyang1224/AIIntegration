namespace AIIntegration.Library.Models
{
    /// <summary>
    /// AI响应模型
    /// </summary>
    public class AIResponse
    {
        /// <summary>
        /// AI生成的文本响应
        /// </summary>
        public string GeneratedText { get; set; }

        /// <summary>
        /// 生成响应时使用的令牌数
        /// </summary>
        public int TokensUsed { get; set; }
    }
}