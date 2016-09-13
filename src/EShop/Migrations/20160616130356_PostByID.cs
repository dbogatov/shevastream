using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Migrations
{
    public partial class PostByID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_BlogPost_Title",
                schema: "shevastream",
                table: "BlogPost");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_BlogPost_TitleUrl",
                schema: "shevastream",
                table: "BlogPost");

            migrationBuilder.AlterColumn<string>(
                name: "TitleUrl",
                schema: "shevastream",
                table: "BlogPost",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "shevastream",
                table: "BlogPost",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TitleUrl",
                schema: "shevastream",
                table: "BlogPost",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "shevastream",
                table: "BlogPost",
                nullable: false);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_BlogPost_Title",
                schema: "shevastream",
                table: "BlogPost",
                column: "Title");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_BlogPost_TitleUrl",
                schema: "shevastream",
                table: "BlogPost",
                column: "TitleUrl");
        }
    }
}
