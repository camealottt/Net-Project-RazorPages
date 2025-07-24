using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Razor_Tutorial_Test.Migrations
{
    /// <inheritdoc />
    public partial class EditItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Items");
        }
    }
}
