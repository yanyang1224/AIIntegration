namespace AIIntegration.Models
{
    public class AIRequest
    {
        public string Prompt { get; set; }
        public int MaxTokens { get; set; }
        // 添加其他通用请求参数
    }
}