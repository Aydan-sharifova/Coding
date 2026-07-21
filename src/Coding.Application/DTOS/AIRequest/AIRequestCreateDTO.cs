using System.ComponentModel.DataAnnotations;
using Coding.Enums;

namespace Coding.DTOS.AIRequest
{
    public class AIRequestCreateDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public AIRequestType Type { get; set; }

        [Required]
        public string Prompt { get; set; } = string.Empty;

        public string? SelectedCode { get; set; }
    }
}
