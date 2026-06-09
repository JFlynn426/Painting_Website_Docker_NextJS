using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerApp.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddIsLandscapeToPainting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLandscape",
                table: "Paintings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLandscape",
                table: "Paintings");
        }
    }
}
