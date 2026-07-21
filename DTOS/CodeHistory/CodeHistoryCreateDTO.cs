namespace Coding.DTOS.CodeHistory
{
    public class CodeHistoryCreateDTO
    {
        public Guid FileItemId { get; set; }
        public Guid UserId { get; set; }
        public string OldContent { get; set; } = string.Empty;
        public string NewContent { get; set; } = string.Empty;
        public DateTime EditedAt { get; set; }
    }
}
