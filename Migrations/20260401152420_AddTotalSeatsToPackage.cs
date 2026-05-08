using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelSphereMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalSeatsToPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalSeats",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalSeats",
                table: "Packages");
        }
    }
}
