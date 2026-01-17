using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNAustria.Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Addresses_LocationId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Organizations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Organizations",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Organizations",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Organizations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Organizations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Zip",
                table: "Organizations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Zip_Latitude_Longitude",
                table: "Organizations",
                columns: new[] { "Zip", "Latitude", "Longitude" });

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Organizations_LocationId",
                table: "Events",
                column: "LocationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Organizations_LocationId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_Zip_Latitude_Longitude",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Zip",
                table: "Organizations");

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    LocationName = table.Column<string>(type: "text", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    State = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    Zip = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Zip_Latitude_Longitude",
                table: "Addresses",
                columns: new[] { "Zip", "Latitude", "Longitude" });

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Addresses_LocationId",
                table: "Events",
                column: "LocationId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
