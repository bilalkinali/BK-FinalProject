using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesforceService.DatabaseMigration.Migrations
{
    /// <inheritdoc />
    public partial class OutboundEntityProperyRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InternalTopic",
                table: "OutboundEvents",
                newName: "SalesforceTopic");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SalesforceTopic",
                table: "OutboundEvents",
                newName: "InternalTopic");
        }
    }
}
