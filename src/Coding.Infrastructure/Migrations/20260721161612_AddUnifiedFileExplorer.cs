using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coding.Migrations
{
    /// <inheritdoc />
    public partial class AddUnifiedFileExplorer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkspaceNodes",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NodeType = table.Column<int>(type: "integer", nullable: false),
                    CreatAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceNodes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkspaceNodes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceNodes_WorkspaceNodes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "WorkspaceNodes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileContents",
                columns: table => new
                {
                    NodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ContentHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileContents", x => x.NodeId);
                    table.ForeignKey(
                        name: "FK_FileContents_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileContents_WorkspaceNodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "WorkspaceNodes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileVersions",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    NodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ContentHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileVersions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FileVersions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileVersions_WorkspaceNodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "WorkspaceNodes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileContents_UpdatedById",
                table: "FileContents",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FileVersions_CreatedById",
                table: "FileVersions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FileVersions_NodeId_VersionNumber",
                table: "FileVersions",
                columns: new[] { "NodeId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceNodes_NodeType",
                table: "WorkspaceNodes",
                column: "NodeType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceNodes_ParentId",
                table: "WorkspaceNodes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceNodes_ProjectId",
                table: "WorkspaceNodes",
                column: "ProjectId");

            migrationBuilder.Sql("""
                CREATE UNIQUE INDEX "IX_WorkspaceNodes_SiblingNameLookup"
                ON "WorkspaceNodes" ("ProjectId", COALESCE("ParentId", '00000000-0000-0000-0000-000000000000'::uuid), lower("Name"))
                WHERE NOT "IsDeleted";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileContents");

            migrationBuilder.DropTable(
                name: "FileVersions");

            migrationBuilder.DropTable(
                name: "WorkspaceNodes");
        }
    }
}
