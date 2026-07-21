namespace Coding.DTOS.GitCommit
{
    public class GitCommitGetDTO
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string CommitMessage { get; set; } = string.Empty;
        public string CommitHash { get; set; } = string.Empty;
        public DateTime CommitDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
