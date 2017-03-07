using Microsoft.EntityFrameworkCore.Migrations;

namespace Shevastream.Migrations
{
    public partial class BlogChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Preview",
                schema: "shevastream",
                table: "BlogPost",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Views",
                schema: "shevastream",
                table: "BlogPost",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Preview",
                schema: "shevastream",
                table: "BlogPost");

            migrationBuilder.DropColumn(
                name: "Views",
                schema: "shevastream",
                table: "BlogPost");
        }
    }
}
