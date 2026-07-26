using Coding.Enums;

namespace Coding.Application.Features.AiAssistant;

public sealed record AiAssistantRequest(
    Guid ProjectId,
    string UserMessage,
    AiAssistantAction Action = AiAssistantAction.Chat,
    Guid? ConversationId = null,
    Guid? CurrentFileId = null,
    string? SelectedCode = null,
    string? NeighboringCode = null,
    string? ProgrammingLanguage = null,
    IReadOnlyList<Guid>? ReferencedFileIds = null);

public sealed record AiRepositoryContext(string Content, int CharacterCount, IReadOnlyList<Guid> IncludedFileIds);
public sealed record AiProviderMessage(AiMessageRole Role, string Content);
public sealed record AiRequest(
    string SystemInstructions,
    string UserInstructions,
    string RepositoryContext,
    string ProgrammingLanguage,
    AiAssistantAction Action,
    IReadOnlyList<AiProviderMessage> History);

public sealed record AiStreamChunk(
    string Content,
    bool IsCompleted = false,
    int? InputTokens = null,
    int? OutputTokens = null,
    string? FinishReason = null,
    Guid? ConversationId = null,
    string? Error = null);
public sealed record AiConversationDto(Guid Id, Guid ProjectId, string Title, DateTime CreatedAt, DateTime UpdatedAt);
public sealed record AiMessageDto(Guid Id, AiMessageRole Role, string Content, AiAssistantAction? Action, Guid? FileId, DateTime CreatedAt);
public sealed record AiConversationDetails(AiConversationDto Conversation, IReadOnlyList<AiMessageDto> Messages);
public sealed record AiUsage(string Provider, string Model, int? InputTokens, int? OutputTokens, decimal? EstimatedCost, int DurationMs, bool WasCancelled);

public interface IAiProvider
{
    string ProviderName { get; }
    string Model { get; }
    IAsyncEnumerable<AiStreamChunk> StreamAsync(AiRequest request, CancellationToken cancellationToken);
}
public interface IAiConversationService
{
    IAsyncEnumerable<AiStreamChunk> StreamAsync(AiAssistantRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<AiConversationDto>> GetConversationsAsync(Guid projectId, CancellationToken cancellationToken);
    Task<AiConversationDetails> GetConversationAsync(Guid conversationId, CancellationToken cancellationToken);
}
public interface IAiContextBuilder
{
    Task<AiRepositoryContext> BuildAsync(AiAssistantRequest request, CancellationToken cancellationToken);
}
public interface IAiPromptTemplateService
{
    string GetSystemInstructions(AiAssistantAction action);
    string BuildUserInstructions(AiAssistantRequest request);
}
public interface IAiUsageTracker
{
    Task TrackAsync(Guid userId, Guid projectId, Guid conversationId, AiUsage usage, CancellationToken cancellationToken);
}
