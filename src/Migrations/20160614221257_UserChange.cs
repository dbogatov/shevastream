using Microsoft.EntityFrameworkCore.Migrations;

namespace Shevastream.Migrations
{
    public partial class UserChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Occupation",
                schema: "shevastream",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                schema: "shevastream",
                table: "User",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Occupation",
                schema: "shevastream",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Position",
                schema: "shevastream",
                table: "User");
        }
    }
}
