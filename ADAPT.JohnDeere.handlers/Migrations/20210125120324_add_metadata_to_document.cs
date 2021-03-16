using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ADAPT.JohnDeere.handlers.Migrations
{
    public partial class add_metadata_to_document : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DownloadUrl",
                schema: "johndeere",
                table: "documentfile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                schema: "johndeere",
                table: "documentfile",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedTime",
                schema: "johndeere",
                table: "documentfile",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SourceMachineSerial",
                schema: "johndeere",
                table: "documentfile",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadUrl",
                schema: "johndeere",
                table: "documentfile");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                schema: "johndeere",
                table: "documentfile");

            migrationBuilder.DropColumn(
                name: "ProcessedTime",
                schema: "johndeere",
                table: "documentfile");

            migrationBuilder.DropColumn(
                name: "SourceMachineSerial",
                schema: "johndeere",
                table: "documentfile");
        }
    }
}
