using Coding.Enums;

namespace Coding.DTOS.AIRequest
{
    public class AIRequestGetDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
        public AIRequestType Type { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public string? SelectedCode { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
