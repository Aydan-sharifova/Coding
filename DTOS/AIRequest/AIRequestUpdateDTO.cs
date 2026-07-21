using Coding.Enums;

namespace Coding.DTOS.AIRequest
{
    public class AIRequestUpdateDTO
    {
        public AIRequestType? Type { get; set; }

        public string? Prompt { get; set; }

        public string? SelectedCode { get; set; }
    }
}
