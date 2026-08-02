using Coding.Enums;

namespace Coding.Models;

/// <summary>
/// Operation kind for a patch produced by the agent.
/// </summary>
public enum AiPatchOperation
{
    Create = 0,
    Modify = 1,
    Rename = 2,
    Delete = 3
}

/// <summary>
/// A validated patch proposed by the agent. The orchestrator checks the
/// original content hash before application and rejects stale patches.
/// </summary>
public sealed class AiPatch : Base
{
    public Guid AgentRunId { get; set; }
    public AiAgentRun AgentRun { get; set; } = null!;

    public Guid? ToolCallId { get; set; }
    public AiToolCall? ToolCall { get; set; }

    /// <summary>Path of the patched file, relative to the project root.</summary>
    public string FilePath { get; set; } = string.Empty;

    public AiPatchOperation Operation { get; set; } = AiPatchOperation.Modify;

    /// <summary>Unified diff payload. Required for Modify/Rename/Delete.</summary>
    public string? UnifiedDiff { get; set; }

    /// <summary>SHA-256 hash of the file content captured when the patch was generated.</summary>
    public string OriginalContentHash { get; set; } = string.Empty;

    /// <summary>SHA-256 hash of the proposed content. Used for fast stale-detection.</summary>
    public string ProposedContentHash { get; set; } = string.Empty;

    public string? Explanation { get; set; }
    public int AddedLineCount { get; set; }
    public int RemovedLineCount { get; set; }

    public AiApprovalStatus ApprovalStatus { get; set; } = AiApprovalStatus.NotRequired;
    public DateTime? ApprovedAt { get; set; }

    public bool Applied { get; set; }
    public DateTime? AppliedAt { get; set; }
    public Guid? AppliedByUserId { get; set; }
    public User? AppliedByUser { get; set; }

    public Guid? FileVersionId { get; set; }
    public Guid? WorkspaceNodeId { get; set; }
}