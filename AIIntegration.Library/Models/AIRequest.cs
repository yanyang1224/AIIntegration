using System.Collections.Generic;

namespace AIIntegration.Library.Models
{
    /// <summary>
    /// AI请求模型
    /// </summary>
    public class AIRequest
    {
        /// <summary>
        /// 对话历史
        /// </summary>
        public List<Message> Messages { get; set; } = new List<Message>();

        /// <summary>
        /// 生成响应的最大令牌数
        /// </summary>
        public int MaxTokens { get; set; }
    }

    /// <summary>
    /// 消息模型
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 消息角色（如 "user" 或 "assistant"）
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
    }
}