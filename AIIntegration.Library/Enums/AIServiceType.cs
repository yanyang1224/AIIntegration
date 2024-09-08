using System.ComponentModel;

namespace AIIntegration.Library.Enums
{
    public enum AIServiceType
    {
        [Description("OpenAI")]
        OpenAI,

        [Description("Claude")]
        Claude,

        [Description("DeepSpeek")]
        DeepSpeek,

        [Description("Qwen")]
        Qwen
    }
}