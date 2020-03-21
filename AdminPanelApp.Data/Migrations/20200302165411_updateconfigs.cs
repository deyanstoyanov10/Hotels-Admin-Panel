using Microsoft.EntityFrameworkCore.Migrations;

namespace AdminPanelApp.Data.Migrations
{
    public partial class updateconfigs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Requisitions_RequisitionId",
                table: "Product");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Requisitions_RequisitionId",
                table: "Product",
                column: "RequisitionId",
                principalTable: "Requisitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Requisitions_RequisitionId",
                table: "Product");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Requisitions_RequisitionId",
                table: "Product",
                column: "RequisitionId",
                principalTable: "Requisitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
