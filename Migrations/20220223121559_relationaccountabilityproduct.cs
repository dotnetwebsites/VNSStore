using Microsoft.EntityFrameworkCore.Migrations;

namespace VNSStoreMgmt.Migrations
{
    public partial class relationaccountabilityproduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Accountabilities_ProductId",
                table: "Accountabilities",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accountabilities_ProductMasters_ProductId",
                table: "Accountabilities",
                column: "ProductId",
                principalTable: "ProductMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accountabilities_ProductMasters_ProductId",
                table: "Accountabilities");

            migrationBuilder.DropIndex(
                name: "IX_Accountabilities_ProductId",
                table: "Accountabilities");
        }
    }
}
