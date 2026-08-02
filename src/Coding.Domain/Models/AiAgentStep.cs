using Coding.Enums;

namespace Coding.Models;

/// <summary>
/// Single step inside an agent run. The orchestrator writes one row per
/// planning, tool call, review, and final report event so the run can be
/// replayed, audited, and resumed after approval.
/// </summary>
public sealed class AiAgentStep : Base
{
    public Guid AgentRunId { get; set; }
    public AiAgentRun AgentRun { get; set; } = null!;

    public int StepNumber { get; set; }
    public AiAgentStepType StepType { get; set; }

    /// <summary>Concise summary of the input that triggered the step.</summary>
    public string? InputSummary { get; set; }

    /// <summary>Concise summary of the output produced by the step.</summary>
    public string? OutputSummary { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public string? ErrorMessage { get; set; }
}