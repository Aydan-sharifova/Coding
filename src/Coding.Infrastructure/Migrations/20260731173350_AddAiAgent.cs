using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coding.Migrations
{
    /// <inheritdoc />
    public partial class AddAiAgent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiAgentRuns",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Mode = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Goal = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CurrentStep = table.Column<int>(type: "integer", nullable: false),
                    MaximumSteps = table.Column<int>(type: "integer", nullable: false),
                    ModelName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    PromptVersion = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    PlanJson = table.Column<string>(type: "jsonb", nullable: true),
                    PlanSummary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiAgentRuns", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AiAgentRuns_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AiAgentRuns_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AiAgentSteps",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    StepNumber = table.Column<int>(type: "integer", nullable: false),
                    StepType = table.Column<int>(type: "integer", nullable: false),
                    InputSummary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    OutputSummary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiAgentSteps", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AiAgentSteps_AiAgentRuns_AgentRunId",
                        column: x => x.AgentRunId,
                        principalTable: "AiAgentRuns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AiReviewFindings",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Line = table.Column<int>(type: "integer", nullable: true),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Recommendation = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiReviewFindings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AiReviewFindings_AiAgentRuns_AgentRunId",
                        column: x => x.AgentRunId,
                        principalTable: "AiAgentRuns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AiToolCalls",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentStepId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToolName = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ArgumentsJson = table.Column<string>(type: "jsonb", nullable: false),
                    RiskLevel = table.Column<int>(type: "integer", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "integer", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExecutedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultSummary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ResultJson = table.Column<string>(type: "jsonb", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiToolCalls", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AiToolCalls_AiAgentRuns_AgentRunId",
                        column: x => x.AgentRunId,
                        principalTable: "AiAgentRuns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AiToolCalls_AiAgentSteps_AgentStepId",
                        column: x => x.AgentStepId,
                        principalTable: "AiAgentSteps",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AiApprovalRequests",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToolCallId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ArgumentsHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiApprovalRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AiApprovalRequests_AiAgentRuns_AgentRunId",
                        column: x => x.AgentRunId,
                        principalTable: "AiAgentRuns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AiApprovalRequests_AiToolCalls_ToolCallId",
                        column: x => x.ToolCallId,
                        principalTable: "AiToolCalls",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AiApprovalRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AiPatches",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToolCallId = table.Column<Guid>(type: "uuid", nullable: true),
                    FilePath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Operation = table.Column<int>(type: "integer", nullable: false),
                    UnifiedDiff = table.Column<string>(type: "text", nullable: true),
                    OriginalContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProposedContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Explanation = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    AddedLineCount = table.Column<int>(type: "integer", nullable: false),
                    RemovedLineCount = table.Column<int>(type: "integer", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "integer", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Applied = table.Column<bool>(type: "boolean", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AppliedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    FileVersionId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkspaceNodeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiPatches", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AiPatches_AiAgentRuns_AgentRunId",
                        column: x => x.AgentRunId,
                        principalTable: "AiAgentRuns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AiPatches_AiToolCalls_ToolCallId",
                        column: x => x.ToolCallId,
                        principalTable: "AiToolCalls",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AiPatches_Users_AppliedByUserId",
                        column: x => x.AppliedByUserId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiAgentRuns_ProjectId_Status_StartedAt",
                table: "AiAgentRuns",
                columns: new[] { "ProjectId", "Status", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AiAgentRuns_UserId_ProjectId_StartedAt",
                table: "AiAgentRuns",
                columns: new[] { "UserId", "ProjectId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AiAgentSteps_AgentRunId_StepNumber",
                table: "AiAgentSteps",
                columns: new[] { "AgentRunId", "StepNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AiApprovalRequests_AgentRunId_Status",
                table: "AiApprovalRequests",
                columns: new[] { "AgentRunId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AiApprovalRequests_ExpiresAt",
                table: "AiApprovalRequests",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_AiApprovalRequests_ToolCallId",
                table: "AiApprovalRequests",
                column: "ToolCallId");

            migrationBuilder.CreateIndex(
                name: "IX_AiApprovalRequests_UserId",
                table: "AiApprovalRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AiPatches_AgentRunId_ApprovalStatus",
                table: "AiPatches",
                columns: new[] { "AgentRunId", "ApprovalStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_AiPatches_AgentRunId_FilePath",
                table: "AiPatches",
                columns: new[] { "AgentRunId", "FilePath" });

            migrationBuilder.CreateIndex(
                name: "IX_AiPatches_AppliedByUserId",
                table: "AiPatches",
                column: "AppliedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AiPatches_ToolCallId",
                table: "AiPatches",
                column: "ToolCallId");

            migrationBuilder.CreateIndex(
                name: "IX_AiReviewFindings_AgentRunId_Severity",
                table: "AiReviewFindings",
                columns: new[] { "AgentRunId", "Severity" });

            migrationBuilder.CreateIndex(
                name: "IX_AiToolCalls_AgentRunId_ApprovalStatus",
                table: "AiToolCalls",
                columns: new[] { "AgentRunId", "ApprovalStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_AiToolCalls_AgentRunId_RequestedAt",
                table: "AiToolCalls",
                columns: new[] { "AgentRunId", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AiToolCalls_AgentStepId",
                table: "AiToolCalls",
                column: "AgentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_AiToolCalls_IdempotencyKey",
                table: "AiToolCalls",
                column: "IdempotencyKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiApprovalRequests");

            migrationBuilder.DropTable(
                name: "AiPatches");

            migrationBuilder.DropTable(
                name: "AiReviewFindings");

            migrationBuilder.DropTable(
                name: "AiToolCalls");

            migrationBuilder.DropTable(
                name: "AiAgentSteps");

            migrationBuilder.DropTable(
                name: "AiAgentRuns");
        }
    }
}
