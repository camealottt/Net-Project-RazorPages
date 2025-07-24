using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Razor_Tutorial_Test.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDisplayOrderToDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Category");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Category");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Category",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
