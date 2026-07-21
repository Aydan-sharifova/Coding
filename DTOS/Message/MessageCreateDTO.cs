namespace Coding.DTOS.Message
{
    public class MessageCreateDTO
    {
        public Guid WorkspaceId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsEdited { get; set; }
    }
}
