using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Coding.Application.Abstractions;
using Coding.Application.Features.AiAssistant;
using Coding.Data;
using Coding.Enums;
using Coding.Exceptions;
using Coding.Infrastructure.Projects;
using Coding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Coding.Infrastructure.AiAssistant;

public sealed class AiConversationService(
    AppDbContext db, ICurrentUser currentUser, IAiProvider provider, IAiContextBuilder contextBuilder,
    IAiPromptTemplateService prompts, IAiUsageTracker usageTracker, IOptions<AiOptions> options) : IAiConversationService
{
    public async IAsyncEnumerable<AiStreamChunk> StreamAsync(AiAssistantRequest request, [EnumeratorCancellation] CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.UserMessage)) throw new FluentValidation.ValidationException("A user message is required.");
        var context = await contextBuilder.BuildAsync(request, ct);
        var conversation = request.ConversationId.HasValue
            ? await db.AiConversations.SingleOrDefaultAsync(x => x.ID == request.ConversationId && x.UserId == currentUser.UserId && x.ProjectId == request.ProjectId, ct) ?? throw new NotFoundException("AI conversation not found.")
            : new AiConversation { ID = Guid.NewGuid(), UserId = currentUser.UserId, ProjectId = request.ProjectId, Title = request.UserMessage.Trim()[..Math.Min(80, request.UserMessage.Trim().Length)], CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, CreatAt = DateTime.UtcNow };
        if (!request.ConversationId.HasValue) db.AiConversations.Add(conversation);
        var userMessage = new AiMessage { ID = Guid.NewGuid(), Conversation = conversation, Role = AiMessageRole.User, Content = request.UserMessage.Trim(), Action = request.Action, FileId = request.CurrentFileId, CreatedAt = DateTime.UtcNow, CreatAt = DateTime.UtcNow };
        db.AiMessages.Add(userMessage); conversation.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(ct);

        var history = await db.AiMessages.AsNoTracking().Where(x => x.ConversationId == conversation.ID).OrderByDescending(x => x.CreatedAt).Take(20).OrderBy(x => x.CreatedAt).Select(x => new AiProviderMessage(x.Role, x.Content)).ToListAsync(ct);
        var providerRequest = new AiRequest(prompts.GetSystemInstructions(request.Action), prompts.BuildUserInstructions(request), context.Content, request.ProgrammingLanguage ?? "plain text", request.Action, history);
        var output = new StringBuilder(); var stopwatch = Stopwatch.StartNew(); int? inputTokens = null; int? outputTokens = null; var cancelled = false;
        yield return new AiStreamChunk(string.Empty, ConversationId: conversation.ID);
        try
        {
            await foreach (var chunk in provider.StreamAsync(providerRequest, ct))
            {
                if (!string.IsNullOrEmpty(chunk.Content)) output.Append(chunk.Content);
                inputTokens = chunk.InputTokens ?? inputTokens; outputTokens = chunk.OutputTokens ?? outputTokens;
                yield return chunk;
            }
        }
        finally
        {
            cancelled = ct.IsCancellationRequested;
            if (output.Length > 0)
            {
                db.AiMessages.Add(new AiMessage { ID = Guid.NewGuid(), ConversationId = conversation.ID, Role = AiMessageRole.Assistant, Content = output.ToString(), Action = request.Action, FileId = request.CurrentFileId, CreatedAt = DateTime.UtcNow, CreatAt = DateTime.UtcNow });
                conversation.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync(CancellationToken.None);
            }
            await usageTracker.TrackAsync(currentUser.UserId, request.ProjectId, conversation.ID, new AiUsage(options.Value.Provider, options.Value.Model, inputTokens, outputTokens, null, (int)stopwatch.ElapsedMilliseconds, cancelled), CancellationToken.None);
        }
    }

    public async Task<IReadOnlyList<AiConversationDto>> GetConversationsAsync(Guid projectId, CancellationToken ct)
    {
        await ProjectAccess.RequireMemberAsync(db, projectId, currentUser.UserId, ct);
        return await db.AiConversations.AsNoTracking().Where(x => x.ProjectId == projectId && x.UserId == currentUser.UserId).OrderByDescending(x => x.UpdatedAt).Select(x => new AiConversationDto(x.ID, x.ProjectId, x.Title, x.CreatedAt, x.UpdatedAt)).ToListAsync(ct);
    }

    public async Task<AiConversationDetails> GetConversationAsync(Guid conversationId, CancellationToken ct)
    {
        var conversation = await db.AiConversations.AsNoTracking().SingleOrDefaultAsync(x => x.ID == conversationId && x.UserId == currentUser.UserId, ct) ?? throw new NotFoundException("AI conversation not found.");
        await ProjectAccess.RequireMemberAsync(db, conversation.ProjectId, currentUser.UserId, ct);
        var messages = await db.AiMessages.AsNoTracking().Where(x => x.ConversationId == conversationId).OrderBy(x => x.CreatedAt).Select(x => new AiMessageDto(x.ID, x.Role, x.Content, x.Action, x.FileId, x.CreatedAt)).ToListAsync(ct);
        return new AiConversationDetails(new(conversation.ID, conversation.ProjectId, conversation.Title, conversation.CreatedAt, conversation.UpdatedAt), messages);
    }
}
