using Microsoft.EntityFrameworkCore.Migrations;

namespace VNSStoreMgmt.Migrations
{
    public partial class productdescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductDestination",
                table: "ProductMasters");

            migrationBuilder.AddColumn<string>(
                name: "ProductDescription",
                table: "ProductMasters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductDescription",
                table: "ProductMasters");

            migrationBuilder.AddColumn<string>(
                name: "ProductDestination",
                table: "ProductMasters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
