namespace Coding.DTOS.Workspace
{
    public class WorkspaceUpdateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public Guid? OwnerId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
