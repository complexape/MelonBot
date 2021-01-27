using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MelonBot.Bots.Migrations
{
    public partial class InitializeSqliteDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    guildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    UserIconURL = table.Column<string>(type: "TEXT", nullable: true),
                    DripScore = table.Column<int>(type: "INTEGER", nullable: false),
                    DripInventory = table.Column<string>(type: "TEXT", nullable: true),
                    DripCooldown = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Tier = table.Column<int>(type: "INTEGER", nullable: false),
                    GambleCount = table.Column<int>(type: "INTEGER", nullable: false),
                    GambleTickets = table.Column<int>(type: "INTEGER", nullable: false),
                    GambleCooldown = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profile");
        }
    }
}
