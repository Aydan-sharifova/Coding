namespace Coding.DTOS.AIResponse
{
    public class AIResponseCreateDTO
    {
        public Guid AIRequestId { get; set; }
        public string ResponseText { get; set; } = string.Empty;
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
