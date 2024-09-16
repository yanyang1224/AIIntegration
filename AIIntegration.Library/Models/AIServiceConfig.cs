namespace AIIntegration.Library.Models
{
    public class AIServiceConfig
    {
        public ServiceConfig OpenAI { get; set; }
        public ServiceConfig Claude { get; set; }
        public ServiceConfig DeepSeek { get; set; }
        public ServiceConfig Qwen { get; set; }
    }

    public class ServiceConfig
    {
        public string ApiUrl { get; set; }
        public string ApiKey { get; set; }
        public string Model { get; set; }
    }
}