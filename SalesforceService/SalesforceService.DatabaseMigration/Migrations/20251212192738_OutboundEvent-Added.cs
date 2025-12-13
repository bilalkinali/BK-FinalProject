using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SalesforceService.DatabaseMigration.Migrations
{
    /// <inheritdoc />
    public partial class OutboundEventAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InboundEvents",
                table: "InboundEvents");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "InboundEvents",
                newName: "CorrelationId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "InboundEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InboundEvents",
                table: "InboundEvents",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OutboundEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CorrelationId = table.Column<string>(type: "text", nullable: false),
                    RecordId = table.Column<string>(type: "text", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboundEvents", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboundEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InboundEvents",
                table: "InboundEvents");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "InboundEvents");

            migrationBuilder.RenameColumn(
                name: "CorrelationId",
                table: "InboundEvents",
                newName: "EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InboundEvents",
                table: "InboundEvents",
                column: "EventId");
        }
    }
}
