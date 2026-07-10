using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServianOps_Backend.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessDetails",
                table: "Sites",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KeysOrCode",
                table: "Sites",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ParkingInformation",
                table: "Sites",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SiteNotes",
                table: "Sites",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessDetails",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "KeysOrCode",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ParkingInformation",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "SiteNotes",
                table: "Sites");
        }
    }
}
