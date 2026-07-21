namespace Coding.DTOS.Workspace
{
    public class WorkspaceCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
