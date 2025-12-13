using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesforceService.DatabaseMigration.Migrations
{
    /// <inheritdoc />
    public partial class ChangedProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InternalTopic",
                table: "OutboundEvents",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalTopic",
                table: "OutboundEvents");
        }
    }
}
