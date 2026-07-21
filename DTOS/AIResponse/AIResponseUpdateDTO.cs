namespace Coding.DTOS.AIResponse
{
    public class AIResponseUpdateDTO
    {
        public Guid? AIRequestId { get; set; }
        public string? ResponseText { get; set; }
        public int? PromptTokens { get; set; }
        public int? CompletionTokens { get; set; }
        public DateTime? GeneratedAt { get; set; }
    }
}
