namespace Coding.DTOS.FileItem
{
    public class FileItemGetDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public long Size { get; set; }
        public Guid FolderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
