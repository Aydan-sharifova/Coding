namespace Coding.DTOS.FileItem
{
    public class FileItemCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public long Size { get; set; }
        public Guid FolderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
