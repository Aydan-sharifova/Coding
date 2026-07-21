namespace Coding.DTOS.Message
{
    public class MessageUpdateDTO
    {
        public Guid? WorkspaceId { get; set; }
        public Guid? SenderId { get; set; }
        public string? Content { get; set; }
        public DateTime? SentAt { get; set; }
        public bool? IsEdited { get; set; }
    }
}
