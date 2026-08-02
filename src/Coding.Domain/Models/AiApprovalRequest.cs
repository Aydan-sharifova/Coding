using Coding.Enums;

namespace Coding.Models;

/// <summary>
/// A pending or completed approval request for a tool call. Approvals are
/// scoped to a single agent run and expire automatically; an approval from
/// one run cannot authorize a tool call in another.
/// </summary>
public sealed class AiApprovalRequest : Base
{
    public Guid AgentRunId { get; set; }
    public AiAgentRun AgentRun { get; set; } = null!;

    public Guid ToolCallId { get; set; }
    public AiToolCall ToolCall { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public AiApprovalStatus Status { get; set; } = AiApprovalStatus.Pending;

    /// <summary>
    /// SHA-256 hash of the tool name and normalized arguments captured at
    /// approval time. The orchestrator re-hashes the executing tool call and
    /// refuses execution if the hashes differ.
    /// </summary>
    public string ArgumentsHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}