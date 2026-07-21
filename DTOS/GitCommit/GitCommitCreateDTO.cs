namespace Coding.DTOS.GitCommit
{
    public class GitCommitCreateDTO
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string CommitMessage { get; set; } = string.Empty;
        public string CommitHash { get; set; } = string.Empty;
        public DateTime CommitDate { get; set; }
    }
}
