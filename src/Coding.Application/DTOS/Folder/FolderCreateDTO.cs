namespace Coding.DTOS.Folder
{
    public class FolderCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public Guid? ParentFolderId { get; set; }
    }
}
