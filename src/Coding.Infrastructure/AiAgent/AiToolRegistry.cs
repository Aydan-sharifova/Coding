using Coding.Application.Features.AiAgent;

namespace Coding.Infrastructure.AiAgent;

/// <summary>
/// DI-backed AI tool registry. Holds the immutable descriptor map and
/// resolves <see cref="IAiTool"/> instances through the per-call service
/// provider so each tool receives its scoped dependencies.
/// </summary>
public sealed class AiToolRegistry : IAiToolRegistry
{
    private readonly IReadOnlyDictionary<string, AiToolDescriptor> _descriptors;
    private readonly IAiToolDescriptorSource _source;

    public AiToolRegistry(IAiToolDescriptorSource source)
    {
        _source = source;
        var map = new Dictionary<string, AiToolDescriptor>(StringComparer.Ordinal);
        foreach (var descriptor in source.GetDescriptors())
        {
            if (string.IsNullOrWhiteSpace(descriptor.Name))
                throw new InvalidOperationException("AI tool descriptors must have a non-empty Name.");
            if (map.ContainsKey(descriptor.Name))
                throw new InvalidOperationException(
                    $"Duplicate AI tool name '{descriptor.Name}'. Tool names must be unique.");
            map[descriptor.Name] = descriptor;
        }
        _descriptors = map;
    }

    public AiToolDescriptor Describe(string toolName)
    {
        if (!_descriptors.TryGetValue(toolName, out var descriptor))
            throw new UnknownAiToolException(toolName);
        return descriptor;
    }

    public bool TryGet(string toolName, out IAiTool tool)
    {
        tool = null!;
        if (!_descriptors.ContainsKey(toolName)) return false;
        tool = _source.Resolve(toolName);
        return tool is not null;
    }

    public IReadOnlyCollection<AiToolDescriptor> ListAll() => _descriptors.Values.ToArray();
}

/// <summary>
/// Resolves descriptors and tool instances. Registered via DI so we can
/// discover all <see cref="IAiTool"/> implementations without a parallel
/// container.
/// </summary>
public interface IAiToolDescriptorSource
{
    IEnumerable<AiToolDescriptor> GetDescriptors();
    IAiTool Resolve(string toolName);
}

/// <summary>
/// Thrown when the orchestrator or planner references an unknown tool name.
/// Mapped to a 400 Bad Request by the global exception handler.
/// </summary>
public sealed class UnknownAiToolException : Exception
{
    public string ToolName { get; }
    public UnknownAiToolException(string toolName)
        : base($"The AI tool '{toolName}' is not registered.")
    {
        ToolName = toolName;
    }
}