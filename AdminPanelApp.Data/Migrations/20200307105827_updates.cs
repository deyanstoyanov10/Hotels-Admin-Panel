using Microsoft.EntityFrameworkCore.Migrations;

namespace AdminPanelApp.Data.Migrations
{
    public partial class updates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hotel",
                table: "UsersInfo",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Packaging",
                table: "Products",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hotel",
                table: "UsersInfo");

            migrationBuilder.DropColumn(
                name: "Packaging",
                table: "Products");
        }
    }
}
