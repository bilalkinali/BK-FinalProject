using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesforceService.DatabaseMigration.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TopicName",
                table: "InboundEvents",
                newName: "SalesforceTopic");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SalesforceTopic",
                table: "InboundEvents",
                newName: "TopicName");
        }

    }
}
