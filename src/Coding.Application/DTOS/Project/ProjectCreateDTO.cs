namespace Coding.DTOS.Project
{
    public class ProjectCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid WorkspaceId { get; set; }
        public string DefaultLanguage { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
