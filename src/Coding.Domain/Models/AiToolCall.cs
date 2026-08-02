using Coding.Enums;

namespace Coding.Models;

/// <summary>
/// Persisted record of a single tool call proposed by the model and
/// (when approved) executed by the application service layer.
/// </summary>
public sealed class AiToolCall : Base
{
    public Guid AgentRunId { get; set; }
    public AiAgentRun AgentRun { get; set; } = null!;

    public Guid? AgentStepId { get; set; }
    public AiAgentStep? AgentStep { get; set; }

    /// <summary>Name of the tool as registered in <see cref="Coding.Application.Abstractions.IAiToolRegistry"/>.</summary>
    public string ToolName { get; set; } = string.Empty;

    /// <summary>Structured tool arguments. JSONB.</summary>
    public string ArgumentsJson { get; set; } = "{}";

    public AiToolRiskLevel RiskLevel { get; set; }

    public AiApprovalStatus ApprovalStatus { get; set; } = AiApprovalStatus.NotRequired;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ExecutedAt { get; set; }

    /// <summary>Concise summary of the result, safe to surface to clients.</summary>
    public string? ResultSummary { get; set; }

    /// <summary>Full structured result for tools that produce JSON output.</summary>
    public string? ResultJson { get; set; }

    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Idempotency key derived from <see cref="AgentRunId"/>, tool name, and
    /// a hash of the normalized arguments. The orchestrator uses it to reject
    /// duplicate execution of the same logical operation.
    /// </summary>
    public string IdempotencyKey { get; set; } = string.Empty;
}