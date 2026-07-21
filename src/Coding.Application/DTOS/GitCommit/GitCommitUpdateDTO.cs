namespace Coding.DTOS.GitCommit
{
    public class GitCommitUpdateDTO
    {
        public Guid? ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public string? CommitMessage { get; set; }
        public string? CommitHash { get; set; }
        public DateTime? CommitDate { get; set; }
    }
}
