using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coding.Migrations
{
    /// <inheritdoc />
    public partial class AddCollaborativeDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CollaborativeDocumentSnapshots",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    EncodedState = table.Column<byte[]>(type: "bytea", nullable: false),
                    StateVector = table.Column<byte[]>(type: "bytea", nullable: false),
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsCompacted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollaborativeDocumentSnapshots", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CollaborativeDocumentSnapshots_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollaborativeDocumentSnapshots_WorkspaceNodes_FileId",
                        column: x => x.FileId,
                        principalTable: "WorkspaceNodes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollaborativeDocumentUpdates",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdateId = table.Column<Guid>(type: "uuid", nullable: false),
                    EncodedUpdate = table.Column<byte[]>(type: "bytea", nullable: false),
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollaborativeDocumentUpdates", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CollaborativeDocumentUpdates_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollaborativeDocumentUpdates_WorkspaceNodes_FileId",
                        column: x => x.FileId,
                        principalTable: "WorkspaceNodes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollaborativeDocumentSnapshots_FileId_SequenceNumber",
                table: "CollaborativeDocumentSnapshots",
                columns: new[] { "FileId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollaborativeDocumentSnapshots_ProjectId",
                table: "CollaborativeDocumentSnapshots",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CollaborativeDocumentUpdates_FileId_SequenceNumber",
                table: "CollaborativeDocumentUpdates",
                columns: new[] { "FileId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollaborativeDocumentUpdates_FileId_UpdateId",
                table: "CollaborativeDocumentUpdates",
                columns: new[] { "FileId", "UpdateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollaborativeDocumentUpdates_ProjectId",
                table: "CollaborativeDocumentUpdates",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollaborativeDocumentSnapshots");

            migrationBuilder.DropTable(
                name: "CollaborativeDocumentUpdates");
        }
    }
}
