using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCarouselPaintingToPaintingConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCarouselPainting",
                table: "Paintings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCarouselPainting",
                table: "Paintings");
        }
    }
}
