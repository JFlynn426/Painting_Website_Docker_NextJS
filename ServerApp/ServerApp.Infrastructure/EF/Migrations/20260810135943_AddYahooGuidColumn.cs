using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerApp.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddYahooGuidColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YahooGuid",
                table: "AdminUsers",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminUsers_YahooGuid",
                table: "AdminUsers",
                column: "YahooGuid",
                unique: true,
                filter: "\"YahooGuid\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdminUsers_YahooGuid",
                table: "AdminUsers");

            migrationBuilder.DropColumn(
                name: "YahooGuid",
                table: "AdminUsers");
        }
    }
}
