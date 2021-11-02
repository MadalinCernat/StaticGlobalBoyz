using Microsoft.EntityFrameworkCore.Migrations;

namespace StaticGlobalBoyz.WebApp.Data.Migrations
{
    public partial class Extend_IdentityUser3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalFirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalLastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalFirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ExternalLastName",
                table: "AspNetUsers");
        }
    }
}
