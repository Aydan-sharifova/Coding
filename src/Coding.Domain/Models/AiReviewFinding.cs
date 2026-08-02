using Coding.Enums;

namespace Coding.Models;

/// <summary>
/// Structured finding produced by the AI reviewer after an agent run
/// completes. The reviewer never modifies files; findings surface only.
/// </summary>
public sealed class AiReviewFinding : Base
{
    public Guid AgentRunId { get; set; }
    public AiAgentRun AgentRun { get; set; } = null!;

    public AiReviewSeverity Severity { get; set; }
    public string Category { get; set; } = string.Empty;

    /// <summary>Optional path of the affected file, relative to the project root.</summary>
    public string? FilePath { get; set; }

    public int? Line { get; set; }

    public string Message { get; set; } = string.Empty;
    public string? Recommendation { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}