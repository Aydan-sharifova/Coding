using System;
namespace Coding.Models
{
    public class GitCommit:Base
    {
        public Guid ProjectId { get; set; }

        public Project Project { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public string CommitMessage { get; set; }

        public string CommitHash { get; set; }

        public DateTime CommitDate { get; set; }
    }
}

