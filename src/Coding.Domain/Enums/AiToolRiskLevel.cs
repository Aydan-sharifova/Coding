namespace Coding.Enums;

/// <summary>
/// Risk classification for AI tool calls. Drives the approval policy.
/// </summary>
public enum AiToolRiskLevel
{
    ReadOnly = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}