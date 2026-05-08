using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelSphereMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdminUIEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BestSeason",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationType",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DifficultyLevel",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "Packages",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DropPoint",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullDescription",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GalleryImages",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HotelName",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HotelRating",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MealsIncluded",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupPoint",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomType",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialAttractions",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TravelType",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestSeason",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "DestinationType",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "DropPoint",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "FullDescription",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "GalleryImages",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "HotelName",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "HotelRating",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "MealsIncluded",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "PickupPoint",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "SpecialAttractions",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "TravelType",
                table: "Packages");
        }
    }
}
