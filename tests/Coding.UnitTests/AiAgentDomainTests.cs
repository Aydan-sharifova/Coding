using Coding.Enums;
using Coding.Models;
using FluentAssertions;
using Xunit;

namespace Coding.UnitTests;

public sealed class AiAgentDomainTests
{
    [Theory]
    [InlineData(AiAgentMode.Ask, 0)]
    [InlineData(AiAgentMode.Plan, 1)]
    [InlineData(AiAgentMode.Agent, 2)]
    [InlineData(AiAgentMode.Review, 3)]
    public void AiAgentMode_has_stable_numeric_values(AiAgentMode mode, int expected)
    {
        ((int)mode).Should().Be(expected);
    }

    [Theory]
    [InlineData(AiAgentStatus.Pending, 0)]
    [InlineData(AiAgentStatus.Planning, 1)]
    [InlineData(AiAgentStatus.WaitingForApproval, 2)]
    [InlineData(AiAgentStatus.Executing, 3)]
    [InlineData(AiAgentStatus.Reviewing, 4)]
    [InlineData(AiAgentStatus.Completed, 5)]
    [InlineData(AiAgentStatus.Failed, 6)]
    [InlineData(AiAgentStatus.Cancelled, 7)]
    public void AiAgentStatus_has_stable_numeric_values(AiAgentStatus status, int expected)
    {
        ((int)status).Should().Be(expected);
    }

    [Fact]
    public void AiApprovalStatus_can_distinguish_pending_from_approved_outcomes()
    {
        AiApprovalStatus.Pending.Should().NotBe(AiApprovalStatus.ApprovedOnce);
        AiApprovalStatus.ApprovedOnce.Should().NotBe(AiApprovalStatus.ApprovedForRun);
        AiApprovalStatus.Rejected.Should().NotBe(AiApprovalStatus.Expired);
        AiApprovalStatus.NotRequired.Should().NotBe(AiApprovalStatus.Pending);
    }

    [Fact]
    public void AiToolCall_initializes_with_safe_defaults()
    {
        var call = new AiToolCall();

        call.ArgumentsJson.Should().Be("{}");
        call.IdempotencyKey.Should().BeEmpty();
        call.ApprovalStatus.Should().Be(AiApprovalStatus.NotRequired);
        call.RiskLevel.Should().Be(AiToolRiskLevel.ReadOnly);
        call.RequestedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void AiPatch_defaults_to_modify_operation_with_zero_diff_counts()
    {
        var patch = new AiPatch();

        patch.Operation.Should().Be(AiPatchOperation.Modify);
        patch.Applied.Should().BeFalse();
        patch.AddedLineCount.Should().Be(0);
        patch.RemovedLineCount.Should().Be(0);
        patch.FilePath.Should().BeEmpty();
    }

    [Fact]
    public void AiAgentRun_defaults_to_pending_status_with_safe_step_limit()
    {
        var run = new AiAgentRun();

        run.Status.Should().Be(AiAgentStatus.Pending);
        run.MaximumSteps.Should().Be(30);
        run.CurrentStep.Should().Be(0);
        run.Goal.Should().BeEmpty();
    }

    [Fact]
    public void AiReviewFinding_carries_required_fields_and_optional_location()
    {
        var finding = new AiReviewFinding
        {
            Severity = AiReviewSeverity.High,
            Category = "Correctness",
            Message = "Off-by-one in the page index.",
            FilePath = null,
            Line = null
        };

        finding.Severity.Should().Be(AiReviewSeverity.High);
        finding.FilePath.Should().BeNull();
        finding.Line.Should().BeNull();
        finding.Recommendation.Should().BeNull();
    }
}