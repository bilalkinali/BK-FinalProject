using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesforceService.DatabaseMigration.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OutboundEvents_CorrelationId",
                table: "OutboundEvents",
                column: "CorrelationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboundEvents_SalesforceTopic_ReplayId",
                table: "InboundEvents",
                columns: new[] { "SalesforceTopic", "ReplayId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboundEvents_CorrelationId",
                table: "OutboundEvents");

            migrationBuilder.DropIndex(
                name: "IX_InboundEvents_SalesforceTopic_ReplayId",
                table: "InboundEvents");
        }
    }
}
