using Microsoft.EntityFrameworkCore.Migrations;

namespace StaticGlobalBoyz.WebApp.Data.Migrations
{
    public partial class Added_Column_HasOrdered : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasOrdered",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasOrdered",
                table: "AspNetUsers");
        }
    }
}
