namespace Coding.DTOS.Folder
{
    public class FolderUpdateDTO
    {
        public string? Name { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? ParentFolderId { get; set; }
    }
}
