using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ADAPT.JohnDeere.handlers.Migrations
{
    public partial class johndeere_add_user_token_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "johndeere");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "johndeere",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    AccessToken = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    RefreshToken = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExpiresIn = table.Column<int>(type: "integer", nullable: false),
                    RegistrationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users",
                schema: "johndeere");
        }
    }
}
