using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerApp.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class MakeGoogleSubjectIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdminUsers_GoogleSubjectId",
                table: "AdminUsers");

            migrationBuilder.AlterColumn<string>(
                name: "GoogleSubjectId",
                table: "AdminUsers",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUsers_GoogleSubjectId",
                table: "AdminUsers",
                column: "GoogleSubjectId",
                unique: true,
                filter: "\"GoogleSubjectId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdminUsers_GoogleSubjectId",
                table: "AdminUsers");

            migrationBuilder.AlterColumn<string>(
                name: "GoogleSubjectId",
                table: "AdminUsers",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminUsers_GoogleSubjectId",
                table: "AdminUsers",
                column: "GoogleSubjectId",
                unique: true);
        }
    }
}
