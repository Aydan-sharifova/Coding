namespace Coding.DTOS.CodeHistory
{
    public class CodeHistoryGetDTO
    {
        public Guid Id { get; set; }
        public Guid FileItemId { get; set; }
        public Guid UserId { get; set; }
        public string OldContent { get; set; } = string.Empty;
        public string NewContent { get; set; } = string.Empty;
        public DateTime EditedAt { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
