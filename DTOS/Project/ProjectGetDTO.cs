namespace Coding.DTOS.Project
{
    public class ProjectGetDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid WorkspaceId { get; set; }
        public string DefaultLanguage { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
