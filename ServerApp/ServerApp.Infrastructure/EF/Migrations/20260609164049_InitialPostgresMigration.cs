using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerApp.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "varchar(256)", nullable: false),
                    DisplayName = table.Column<string>(type: "varchar(100)", nullable: false),
                    PictureUrl = table.Column<string>(type: "varchar(500)", nullable: true),
                    GoogleSubjectId = table.Column<string>(type: "varchar(100)", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "varchar(200)", nullable: false),
                    Title = table.Column<string>(type: "varchar(200)", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    PhotoUrls = table.Column<string>(type: "jsonb", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaintingCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Slug = table.Column<string>(type: "varchar(100)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaintingCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Paintings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "varchar(200)", nullable: false),
                    Slug = table.Column<string>(type: "varchar(200)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "varchar(500)", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "varchar(500)", nullable: true),
                    CategorySlug = table.Column<string>(type: "varchar(100)", nullable: false),
                    Width = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Height = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Depth = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paintings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paintings_PaintingCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "PaintingCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminUsers_Email",
                table: "AdminUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminUsers_GoogleSubjectId",
                table: "AdminUsers",
                column: "GoogleSubjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_Address",
                table: "PageContents",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaintingCategories_Slug",
                table: "PaintingCategories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Paintings_CategoryId",
                table: "Paintings",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Paintings_CategorySlug",
                table: "Paintings",
                column: "CategorySlug");

            migrationBuilder.CreateIndex(
                name: "IX_Paintings_Slug",
                table: "Paintings",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminUsers");

            migrationBuilder.DropTable(
                name: "PageContents");

            migrationBuilder.DropTable(
                name: "Paintings");

            migrationBuilder.DropTable(
                name: "PaintingCategories");
        }
    }
}
