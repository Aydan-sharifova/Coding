using Coding.Enums;

namespace Coding.Models;

/// <summary>
/// Persisted record of an agent run inside a project. Holds run-level
/// metadata, lifecycle state, and aggregate counters that the orchestrator
/// uses to enforce execution limits.
/// </summary>
public sealed class AiAgentRun : Base
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public AiAgentMode Mode { get; set; }
    public AiAgentStatus Status { get; set; } = AiAgentStatus.Pending;

    /// <summary>User-authored development goal that started the run.</summary>
    public string Goal { get; set; } = string.Empty;

    public int CurrentStep { get; set; }
    public int MaximumSteps { get; set; } = 30;

    public string? ModelName { get; set; }
    public string? PromptVersion { get; set; }

    /// <summary>Structured plan produced by the planner. JSONB.</summary>
    public string? PlanJson { get; set; }

    /// <summary>Concise summary of the plan, safe to surface in timelines.</summary>
    public string? PlanSummary { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }

    /// <summary>Set when the run is cancelled.</summary>
    public DateTime? CancelledAt { get; set; }

    public ICollection<AiAgentStep> Steps { get; set; } = [];
    public ICollection<AiToolCall> ToolCalls { get; set; } = [];
    public ICollection<AiApprovalRequest> Approvals { get; set; } = [];
    public ICollection<AiPatch> Patches { get; set; } = [];
    public ICollection<AiReviewFinding> Findings { get; set; } = [];
}