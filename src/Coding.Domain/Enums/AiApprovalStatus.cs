namespace Coding.Enums;

/// <summary>
/// State of an approval request for a single tool call inside an agent run.
/// </summary>
public enum AiApprovalStatus
{
    NotRequired = 0,
    Pending = 1,
    ApprovedOnce = 2,
    ApprovedForRun = 3,
    Rejected = 4,
    Expired = 5
}