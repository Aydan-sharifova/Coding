using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coding.Migrations
{
    /// <inheritdoc />
    public partial class GlobalSearchAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_Projects_SearchTrgm" ON "Projects" USING gin (lower("Name") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_Projects_SearchText" ON "Projects" USING gin (to_tsvector('simple', coalesce("Name",'') || ' ' || coalesce("Description",'')));""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_WorkspaceNodes_SearchTrgm" ON "WorkspaceNodes" USING gin (lower("Name") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_ProjectTasks_SearchTrgm" ON "ProjectTasks" USING gin (lower("Title") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_ProjectTasks_SearchText" ON "ProjectTasks" USING gin (to_tsvector('simple', coalesce("Title",'') || ' ' || coalesce("Description",'')));""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_Users_UserName_SearchTrgm" ON "Users" USING gin (lower("UserName") gin_trgm_ops);""");

            migrationBuilder.CreateTable(
                name: "CodingSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastActivityAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodingSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodingSessions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CodingSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CodingSessions_WorkspaceNodes_FileId",
                        column: x => x.FileId,
                        principalTable: "WorkspaceNodes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodingSessions_FileId_StartAt",
                table: "CodingSessions",
                columns: new[] { "FileId", "StartAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CodingSessions_ProjectId_StartAt",
                table: "CodingSessions",
                columns: new[] { "ProjectId", "StartAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CodingSessions_UserId_FileId_EndAt",
                table: "CodingSessions",
                columns: new[] { "UserId", "FileId", "EndAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CodingSessions_UserId_StartAt",
                table: "CodingSessions",
                columns: new[] { "UserId", "StartAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_Users_UserName_SearchTrgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_ProjectTasks_SearchText";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_ProjectTasks_SearchTrgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_WorkspaceNodes_SearchTrgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_Projects_SearchText";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_Projects_SearchTrgm";""");

            migrationBuilder.DropTable(
                name: "CodingSessions");
        }
    }
}
