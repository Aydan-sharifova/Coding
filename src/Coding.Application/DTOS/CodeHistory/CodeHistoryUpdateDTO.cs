namespace Coding.DTOS.CodeHistory
{
    public class CodeHistoryUpdateDTO
    {
        public Guid? FileItemId { get; set; }
        public Guid? UserId { get; set; }
        public string? OldContent { get; set; }
        public string? NewContent { get; set; }
        public DateTime? EditedAt { get; set; }
    }
}
