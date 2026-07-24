namespace Coding.Infrastructure.AiAssistant;

public sealed class AiOptions
{
    public const string SectionName = "Ai";
    public string Provider { get; set; } = "Development";
    public string Model { get; set; } = "development-assistant";
    public string? ApiKey { get; set; }
    public string? Endpoint { get; set; }
    public int MaxContextCharacters { get; set; } = 32000;
}
