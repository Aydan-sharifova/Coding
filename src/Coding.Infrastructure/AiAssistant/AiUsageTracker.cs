using Coding.Application.Features.AiAssistant;
using Coding.Data;
using Coding.Models;

namespace Coding.Infrastructure.AiAssistant;

public sealed class AiUsageTracker(AppDbContext db) : IAiUsageTracker
{
    public async Task TrackAsync(Guid userId, Guid projectId, Guid conversationId, AiUsage usage, CancellationToken ct)
    {
        db.AiUsageRecords.Add(new AiUsageRecord { Id = Guid.NewGuid(), UserId = userId, ProjectId = projectId, ConversationId = conversationId, Provider = usage.Provider, Model = usage.Model, InputTokens = usage.InputTokens, OutputTokens = usage.OutputTokens, EstimatedCost = usage.EstimatedCost, DurationMs = usage.DurationMs, WasCancelled = usage.WasCancelled, CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync(ct);
    }
}
