namespace Coding.Infrastructure.AiAssistant;

public sealed class AiProviderOptions
{
    public const string SectionName = "AI";
    public string Provider { get; init; } = "Development";
}

public sealed class OpenAiCompatibleOptions
{
    public const string SectionName = "OpenAICompatible";
    public string BaseUrl { get; init; } = "http://localhost:11434/v1/";
    public string Model { get; init; } = "qwen2.5-coder:1.5b";
    public string VisionModel { get; init; } = string.Empty;
    public string ApiKey { get; init; } = "ollama";
    public int MaxOutputTokens { get; init; } = 2_048;
    public double Temperature { get; init; } = 0.1;
}
