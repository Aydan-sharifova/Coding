namespace Coding.DTOS.FileItem
{
    public class FileItemUpdateDTO
    {
        public string? Name { get; set; }
        public string? Extension { get; set; }
        public string? Content { get; set; }
        public long? Size { get; set; }
        public Guid? FolderId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
