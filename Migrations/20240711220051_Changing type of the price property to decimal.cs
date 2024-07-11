using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apartment_Marketplace_API.Migrations
{
    /// <inheritdoc />
    public partial class Changingtypeofthepricepropertytodecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Apartments",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<uint>(
                name: "Price",
                table: "Apartments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");
        }
    }
}
