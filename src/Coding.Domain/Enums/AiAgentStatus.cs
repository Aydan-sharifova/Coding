namespace Coding.Enums;

/// <summary>
/// Lifecycle state of an agent run.
/// </summary>
public enum AiAgentStatus
{
    Pending = 0,
    Planning = 1,
    WaitingForApproval = 2,
    Executing = 3,
    Reviewing = 4,
    Completed = 5,
    Failed = 6,
    Cancelled = 7
}