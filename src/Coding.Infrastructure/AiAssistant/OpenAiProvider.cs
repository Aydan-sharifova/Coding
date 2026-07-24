using Coding.Application.Features.AiAssistant;
using Microsoft.Extensions.Options;

namespace Coding.Infrastructure.AiAssistant;

// Provider placeholder only. It deliberately has no dependency on an OpenAI SDK.
public sealed class OpenAiProvider(IOptions<AiOptions> options) : IAiProvider
{
    public async IAsyncEnumerable<AiStreamChunk> StreamAsync(AiRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(options.Value.ApiKey))
            throw new InvalidOperationException("AI provider credentials are not configured. Set Ai:ApiKey through a secure configuration source.");
        throw new NotSupportedException("The OpenAI provider adapter is a placeholder. Implement its HTTP/SDK transport without changing Application abstractions.");
#pragma warning disable CS0162
        yield break;
#pragma warning restore CS0162
    }
}
