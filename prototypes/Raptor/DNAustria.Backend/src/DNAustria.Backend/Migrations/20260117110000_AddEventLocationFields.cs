using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNAustria.Backend.Migrations
{
    public partial class AddEventLocationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationStreet",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCity",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationZip",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationState",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LocationLatitude",
                table: "Events",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LocationLongitude",
                table: "Events",
                type: "double precision",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationStreet",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationCity",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationZip",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationState",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationLatitude",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationLongitude",
                table: "Events");
        }
    }
}
