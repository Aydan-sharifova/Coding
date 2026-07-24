using System.Text;
using Coding.Application.Abstractions;
using Coding.Application.Features.AiAssistant;
using Coding.Data;
using Coding.Exceptions;
using Coding.Infrastructure.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Coding.Infrastructure.AiAssistant;

public sealed class AiContextBuilder(AppDbContext db, ICurrentUser currentUser, IOptions<AiOptions> options) : IAiContextBuilder
{
    public async Task<AiRepositoryContext> BuildAsync(AiAssistantRequest request, CancellationToken ct)
    {
        await ProjectAccess.RequireMemberAsync(db, request.ProjectId, currentUser.UserId, ct);
        var limit = Math.Clamp(options.Value.MaxContextCharacters, 4000, 100000);
        var output = new StringBuilder(limit); var included = new List<Guid>();
        Append(output, "SELECTED CODE", request.SelectedCode, Math.Min(12000, limit));
        Append(output, "NEIGHBORING CODE", request.NeighboringCode, Math.Min(6000, Math.Max(0, limit - output.Length)));

        var ids = new[] { request.CurrentFileId }.Where(x => x.HasValue).Select(x => x!.Value)
            .Concat(request.ReferencedFileIds ?? []).Distinct().Take(6).ToArray();
        if (ids.Length > 0)
        {
            var files = await db.WorkspaceNodes.AsNoTracking().Where(x => ids.Contains(x.ID) && x.ProjectId == request.ProjectId && x.NodeType == Coding.Enums.WorkspaceNodeType.File)
                .Select(x => new { x.ID, x.Name, Content = x.FileContent == null ? string.Empty : x.FileContent.Content }).ToListAsync(ct);
            if (files.Count != ids.Length) throw new ForbiddenException("One or more requested files are unavailable in this project.");
            foreach (var id in ids)
            {
                var file = files.Single(x => x.ID == id); var remaining = limit - output.Length; if (remaining <= 100) break;
                Append(output, $"FILE: {file.Name}", file.Content, remaining); included.Add(file.ID);
            }
        }
        return new AiRepositoryContext(output.ToString(), output.Length, included);
    }

    private static void Append(StringBuilder target, string label, string? value, int budget)
    {
        if (string.IsNullOrWhiteSpace(value) || budget <= label.Length + 8) return;
        var text = value.Length > budget - label.Length - 8 ? value[..(budget - label.Length - 8)] : value;
        target.AppendLine($"--- {label} (UNTRUSTED REPOSITORY DATA) ---").AppendLine(text).AppendLine("--- END CONTEXT ---");
    }
}
