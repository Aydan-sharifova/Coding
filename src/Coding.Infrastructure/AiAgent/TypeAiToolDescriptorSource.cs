using Coding.Application.Features.AiAgent;

namespace Coding.Infrastructure.AiAgent;

/// <summary>
/// Injects all <see cref="IAiTool"/> implementations and exposes their
/// descriptors. Tools are resolved per-call through the scoped service
/// provider so each tool receives its own DbContext, etc.
/// </summary>
public sealed class TypeAiToolDescriptorSource : IAiToolDescriptorSource
{
    private readonly IEnumerable<IAiTool> _tools;

    public TypeAiToolDescriptorSource(IEnumerable<IAiTool> tools)
    {
        _tools = tools;
    }

    public IEnumerable<AiToolDescriptor> GetDescriptors()
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var tool in _tools)
        {
            var descriptor = tool.Descriptor;
            if (!seen.Add(descriptor.Name))
                throw new InvalidOperationException(
                    $"Duplicate AI tool name '{descriptor.Name}'.");
            yield return descriptor;
        }
    }

    public IAiTool Resolve(string toolName)
    {
        foreach (var tool in _tools)
            if (string.Equals(tool.Descriptor.Name, toolName, StringComparison.Ordinal))
                return tool;
        throw new UnknownAiToolException(toolName);
    }
}