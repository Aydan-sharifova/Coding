using Coding.Enums;
using Coding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coding.Data;

public sealed class AiAgentRunConfiguration : IEntityTypeConfiguration<AiAgentRun>
{
    public void Configure(EntityTypeBuilder<AiAgentRun> builder)
    {
        builder.HasQueryFilter(run => !run.IsDeleted && !run.Project.IsDeleted);

        builder.Property(run => run.Goal).HasMaxLength(2000).IsRequired();
        builder.Property(run => run.ModelName).HasMaxLength(120);
        builder.Property(run => run.PromptVersion).HasMaxLength(40);
        builder.Property(run => run.PlanJson).HasColumnType("jsonb");
        builder.Property(run => run.PlanSummary).HasMaxLength(2000);
        builder.Property(run => run.ErrorMessage).HasMaxLength(2000);

        builder.HasIndex(run => new { run.UserId, run.ProjectId, run.StartedAt });
        builder.HasIndex(run => new { run.ProjectId, run.Status, run.StartedAt });

        builder.HasOne(run => run.User).WithMany().HasForeignKey(run => run.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(run => run.Project).WithMany().HasForeignKey(run => run.ProjectId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class AiAgentStepConfiguration : IEntityTypeConfiguration<AiAgentStep>
{
    public void Configure(EntityTypeBuilder<AiAgentStep> builder)
    {
        builder.HasQueryFilter(step => !step.IsDeleted && !step.AgentRun.IsDeleted && !step.AgentRun.Project.IsDeleted);

        builder.Property(step => step.InputSummary).HasMaxLength(2000);
        builder.Property(step => step.OutputSummary).HasMaxLength(2000);
        builder.Property(step => step.ErrorMessage).HasMaxLength(2000);

        builder.HasIndex(step => new { step.AgentRunId, step.StepNumber }).IsUnique();

        builder.HasOne(step => step.AgentRun).WithMany(run => run.Steps).HasForeignKey(step => step.AgentRunId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class AiToolCallConfiguration : IEntityTypeConfiguration<AiToolCall>
{
    public void Configure(EntityTypeBuilder<AiToolCall> builder)
    {
        builder.HasQueryFilter(call => !call.IsDeleted && !call.AgentRun.IsDeleted && !call.AgentRun.Project.IsDeleted);

        builder.Property(call => call.ToolName).HasMaxLength(80).IsRequired();
        builder.Property(call => call.ArgumentsJson).HasColumnType("jsonb");
        builder.Property(call => call.ResultSummary).HasMaxLength(2000);
        builder.Property(call => call.ResultJson).HasColumnType("jsonb");
        builder.Property(call => call.ErrorMessage).HasMaxLength(2000);
        builder.Property(call => call.IdempotencyKey).HasMaxLength(128).IsRequired();

        builder.HasIndex(call => call.IdempotencyKey).IsUnique();
        builder.HasIndex(call => new { call.AgentRunId, call.RequestedAt });
        builder.HasIndex(call => new { call.AgentRunId, call.ApprovalStatus });

        builder.HasOne(call => call.AgentRun).WithMany(run => run.ToolCalls).HasForeignKey(call => call.AgentRunId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(call => call.AgentStep).WithMany().HasForeignKey(call => call.AgentStepId).OnDelete(DeleteBehavior.SetNull);
    }
}

public sealed class AiApprovalRequestConfiguration : IEntityTypeConfiguration<AiApprovalRequest>
{
    public void Configure(EntityTypeBuilder<AiApprovalRequest> builder)
    {
        builder.HasQueryFilter(approval => !approval.IsDeleted && !approval.AgentRun.IsDeleted && !approval.AgentRun.Project.IsDeleted);

        builder.Property(approval => approval.ArgumentsHash).HasMaxLength(128).IsRequired();

        builder.HasIndex(approval => new { approval.AgentRunId, approval.Status });
        builder.HasIndex(approval => approval.ExpiresAt);

        builder.HasOne(approval => approval.AgentRun).WithMany(run => run.Approvals).HasForeignKey(approval => approval.AgentRunId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(approval => approval.ToolCall).WithMany().HasForeignKey(approval => approval.ToolCallId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(approval => approval.User).WithMany().HasForeignKey(approval => approval.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AiPatchConfiguration : IEntityTypeConfiguration<AiPatch>
{
    public void Configure(EntityTypeBuilder<AiPatch> builder)
    {
        builder.HasQueryFilter(patch => !patch.IsDeleted && !patch.AgentRun.IsDeleted && !patch.AgentRun.Project.IsDeleted);

        builder.Property(patch => patch.FilePath).HasMaxLength(1024).IsRequired();
        builder.Property(patch => patch.UnifiedDiff).HasColumnType("text");
        builder.Property(patch => patch.OriginalContentHash).HasMaxLength(128).IsRequired();
        builder.Property(patch => patch.ProposedContentHash).HasMaxLength(128).IsRequired();
        builder.Property(patch => patch.Explanation).HasMaxLength(4000);

        builder.HasIndex(patch => new { patch.AgentRunId, patch.ApprovalStatus });
        builder.HasIndex(patch => new { patch.AgentRunId, patch.FilePath });

        builder.HasOne(patch => patch.AgentRun).WithMany(run => run.Patches).HasForeignKey(patch => patch.AgentRunId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(patch => patch.ToolCall).WithMany().HasForeignKey(patch => patch.ToolCallId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(patch => patch.AppliedByUser).WithMany().HasForeignKey(patch => patch.AppliedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AiReviewFindingConfiguration : IEntityTypeConfiguration<AiReviewFinding>
{
    public void Configure(EntityTypeBuilder<AiReviewFinding> builder)
    {
        builder.HasQueryFilter(finding => !finding.IsDeleted && !finding.AgentRun.IsDeleted && !finding.AgentRun.Project.IsDeleted);

        builder.Property(finding => finding.Category).HasMaxLength(80).IsRequired();
        builder.Property(finding => finding.FilePath).HasMaxLength(1024);
        builder.Property(finding => finding.Message).HasMaxLength(2000).IsRequired();
        builder.Property(finding => finding.Recommendation).HasMaxLength(4000);

        builder.HasIndex(finding => new { finding.AgentRunId, finding.Severity });

        builder.HasOne(finding => finding.AgentRun).WithMany(run => run.Findings).HasForeignKey(finding => finding.AgentRunId).OnDelete(DeleteBehavior.Cascade);
    }
}