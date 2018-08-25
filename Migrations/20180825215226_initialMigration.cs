using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCoreBugRepro.Migrations
{
    public partial class initialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReproEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MyTime = table.Column<DateTime>(type: "smalldatetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReproEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkingEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MyTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingEntity", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReproEntity");

            migrationBuilder.DropTable(
                name: "WorkingEntity");
        }
    }
}
