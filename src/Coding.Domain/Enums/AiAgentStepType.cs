namespace Coding.Enums;

/// <summary>
/// Classification for a single step executed inside an agent run.
/// </summary>
public enum AiAgentStepType
{
    Planning = 0,
    ToolCall = 1,
    Review = 2,
    FinalReport = 3
}