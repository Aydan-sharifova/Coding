namespace Coding.DTOS.Project
{
    public class ProjectUpdateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? WorkspaceId { get; set; }
        public string? DefaultLanguage { get; set; }
        public bool? IsPublic { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
