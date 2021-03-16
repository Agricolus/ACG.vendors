using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ADAPT.JohnDeere.handlers.Migrations
{
    public partial class add_johndeere_machines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "machines",
                schema: "johndeere",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", maxLength: 64, nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    ExternalId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    SyncTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RegistrationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_machines", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "machines",
                schema: "johndeere");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
        }
    }
}
