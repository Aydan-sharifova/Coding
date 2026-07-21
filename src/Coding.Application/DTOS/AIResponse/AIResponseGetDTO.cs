namespace Coding.DTOS.AIResponse
{
    public class AIResponseGetDTO
    {
        public Guid Id { get; set; }
        public Guid AIRequestId { get; set; }
        public string ResponseText { get; set; } = string.Empty;
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
