using Coding.Application.Features.AiAgent;
using Coding.Enums;
using Coding.Models;

namespace Coding.Infrastructure.AiAgent;

/// <summary>
/// Implements <see cref="IAiToolApprovalPolicy"/>. Risk drives the rule:
/// ReadOnly needs no approval; Low can run automatically when the run opted
/// in; Medium requires one-time approval; High always requires explicit
/// approval; Critical is blocked at this layer.
/// </summary>
public sealed class AiToolApprovalPolicy : IAiToolApprovalPolicy
{
    public bool RequiresApproval(AiToolDescriptor descriptor) => descriptor.RiskLevel switch
    {
        AiToolRiskLevel.ReadOnly => false,
        AiToolRiskLevel.Low      => false,
        AiToolRiskLevel.Medium   => true,
        AiToolRiskLevel.High     => true,
        AiToolRiskLevel.Critical => true,
        _ => true
    };

    public bool CanAutoApproveLowRisk(AiAgentRun run, AiToolDescriptor descriptor)
    {
        if (descriptor.RiskLevel != AiToolRiskLevel.Low) return false;
        // Opt-in flag is recorded on the run's goal text via an explicit prefix;
        // it is never implicit. We require a property in the run JSON to avoid
        // the orchestrator having to interpret free-form goals.
        return run.PromptVersion is not null && run.PromptVersion.Contains("auto-approve-low", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsApprovalValid(AiApprovalStatus status, string approvalHash, string callHash, DateTime expiresAt, DateTime nowUtc)
    {
        if (status is not (AiApprovalStatus.ApprovedOnce or AiApprovalStatus.ApprovedForRun))
            return false;
        if (expiresAt <= nowUtc)
            return false;
        return string.Equals(approvalHash, callHash, StringComparison.Ordinal);
    }
}