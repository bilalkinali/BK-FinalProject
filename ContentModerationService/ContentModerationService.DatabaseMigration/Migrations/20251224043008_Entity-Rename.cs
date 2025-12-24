using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ContentModerationService.DatabaseMigration.Migrations
{
    /// <inheritdoc />
    public partial class EntityRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ModerationDecisions",
                newName: "ModerationResults");

            migrationBuilder.RenameIndex(
                name: "IX_ModerationDecisions_CorrelationId",
                table: "ModerationResults",
                newName: "IX_ModerationResults_CorrelationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ModerationResults",
                newName: "ModerationDecisions");

            migrationBuilder.RenameIndex(
                name: "IX_ModerationResults_CorrelationId",
                table: "ModerationDecisions",
                newName: "IX_ModerationDecisions_CorrelationId");
        }

    }
}
