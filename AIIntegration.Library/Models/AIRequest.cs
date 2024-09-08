namespace AIIntegration.Library.Models
{
    /// <summary>
    /// AI请求模型
    /// </summary>
    public class AIRequest
    {
        /// <summary>
        /// AI生成的提示文本
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// 生成响应的最大令牌数
        /// </summary>
        public int MaxTokens { get; set; }
    }
}