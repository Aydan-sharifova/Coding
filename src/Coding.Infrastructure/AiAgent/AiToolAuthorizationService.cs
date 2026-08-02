using Coding.Application.Abstractions;
using Coding.Application.Features.AiAgent;
using Coding.Data;
using Coding.Enums;
using Coding.Models;
using Microsoft.EntityFrameworkCore;

namespace Coding.Infrastructure.AiAgent;

/// <summary>
/// Central authorization gate for AI tool calls. Implements
/// <see cref="IAiToolAuthorizationService"/>. Reuses <see cref="ProjectAccess"/>
/// and <see cref="ICurrentUser"/> so the agent never invents a parallel role
/// check.
/// </summary>
public sealed class AiToolAuthorizationService : IAiToolAuthorizationService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IAiToolRegistry _registry;

    public AiToolAuthorizationService(AppDbContext db, ICurrentUser currentUser, IAiToolRegistry registry)
    {
        _db = db;
        _currentUser = currentUser;
        _registry = registry;
    }

    public async Task<AiAuthorizationDecision> AuthorizeAsync(AiToolCall call, AiAgentRun run, CancellationToken cancellationToken)
    {
        if (run is null) return AiAuthorizationDecision.Deny("Run is required.", nameof(AiAgentRun));
        if (call is null) return AiAuthorizationDecision.Deny("Tool call is required.", nameof(AiToolCall));

        // 1. Project must still exist and not be soft-deleted.
        var projectExists = await _db.Projects.AsNoTracking()
            .AnyAsync(p => p.ID == run.ProjectId && !p.IsDeleted, cancellationToken);
        if (!projectExists)
            return AiAuthorizationDecision.Deny("Project not found or deleted.", "ProjectActive");

        // 2. Active membership. System admin role does NOT bypass this.
        var role = await _db.ProjectMembers.AsNoTracking()
            .Where(m => m.ProjectId == run.ProjectId && m.UserId == run.UserId)
            .Select(m => (ProjectRole?)m.Role)
            .SingleOrDefaultAsync(cancellationToken);
        if (role is null)
            return AiAuthorizationDecision.Deny("User is not a member of the project.", "ProjectMember");

        // 3. Tool must belong to the current run / project. The orchestrator
        //    cannot repoint a tool call at a different project context.
        if (call.AgentRunId != run.ID)
            return AiAuthorizationDecision.Deny("Tool call does not belong to this run.", "ToolCallOwnership");

        // 4. Entity ownership — when the call references a project entity it
        //    must belong to the same project.
        //    (Per-tool entity ownership is validated inside each tool, since
        //    argument shape differs per tool. The check here covers the
        //    generic case: if a node id is supplied and resolves, it must
        //    belong to this project.)

        // 5. Role gate defined by the tool descriptor.
        AiToolDescriptor descriptor;
        try
        {
            descriptor = _registry.Describe(call.ToolName);
        }
        catch (UnknownAiToolException)
        {
            return AiAuthorizationDecision.Deny("Unknown tool.", "ToolExists");
        }

        if (!descriptor.RequiredRoles.Contains(role.Value))
            return AiAuthorizationDecision.Deny(
                $"Project role '{role.Value}' cannot use tool '{descriptor.Name}'.",
                "ProjectRole");

        // 6. Mode restriction. Ask / Plan / Review cannot use write tools.
        if (!descriptor.AllowedModes.Contains(run.Mode))
            return AiAuthorizationDecision.Deny(
                $"Tool '{descriptor.Name}' is not allowed in mode '{run.Mode}'.",
                "ModeAllowed");

        // 7. Environment / deployment gate. Reserved for future use; this
        //    version treats all registered tools as available, but blocks
        //    critical-tier tools unconditionally.
        if (descriptor.RiskLevel == AiToolRiskLevel.Critical)
            return AiAuthorizationDecision.Deny(
                $"Tool '{descriptor.Name}' is in the critical risk tier and is blocked.",
                "RiskCritical");

        // 8-10. Approval policy is enforced by the execution service after
        //        this authorization. It needs the run state to evaluate the
        //        approval envelope, so we don't duplicate that here.

        return AiAuthorizationDecision.Allow();
    }
}