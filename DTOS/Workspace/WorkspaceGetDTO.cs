namespace Coding.DTOS.Workspace
{
    public class WorkspaceGetDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
