namespace Coding.Enums;

/// <summary>
/// AI assistant capability modes. Each mode controls which tools are reachable
/// for an agent run and how they may be executed.
/// </summary>
public enum AiAgentMode
{
    Ask = 0,
    Plan = 1,
    Agent = 2,
    Review = 3
}