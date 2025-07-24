using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Razor_Tutorial_Test.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderRecordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuyerUserID = table.Column<int>(type: "int", nullable: false),
                    SellerUserID = table.Column<int>(type: "int", nullable: false),
                    OfferedItemID = table.Column<int>(type: "int", nullable: true),
                    RequestedItemID = table.Column<int>(type: "int", nullable: false),
                    MoneyOffered = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRecord", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderRecord");
        }
    }
}
