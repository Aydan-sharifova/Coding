using System;
using Coding.Enums;
using Coding.Models;

namespace Coding.Models
{
    public class AIRequest:Base
    {
      

        public Guid UserId { get; set; }

        public User User { get; set; }

        public Guid ProjectId { get; set; }

        public Project Project { get; set; }

        public AIRequestType Type { get; set; }

        public string Prompt { get; set; }

        public string? SelectedCode { get; set; }

        public DateTime RequestedAt { get; set; }

        public ICollection<AIResponse> Responses { get; set; }
    }
}

