using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VNSStoreMgmt.Migrations
{
    public partial class Barcodelist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductMasters_ProductBarcode",
                table: "ProductMasters");

            migrationBuilder.DropIndex(
                name: "IX_ProductMasters_ProductCode",
                table: "ProductMasters");

            migrationBuilder.DropIndex(
                name: "IX_ProductMasters_SerialNo",
                table: "ProductMasters");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNo",
                table: "ProductMasters",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "ProductMasters",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductCode",
                table: "ProductMasters",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductBarcode",
                table: "ProductMasters",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "BarcodeLists",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    BMId = table.Column<int>(nullable: false),
                    Barcode = table.Column<string>(nullable: true),
                    IsUsedBarcode = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodeLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BarcodeMasters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    BarcodePrintCount = table.Column<int>(nullable: false),
                    BarcodeFrom = table.Column<long>(nullable: true),
                    BarcodeTo = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodeMasters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasters_ProductBarcode",
                table: "ProductMasters",
                column: "ProductBarcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasters_ProductCode",
                table: "ProductMasters",
                column: "ProductCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasters_SerialNo",
                table: "ProductMasters",
                column: "SerialNo",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarcodeLists");

            migrationBuilder.DropTable(
                name: "BarcodeMasters");

            migrationBuilder.DropIndex(
                name: "IX_ProductMasters_ProductBarcode",
                table: "ProductMasters");

            migrationBuilder.DropIndex(
                name: "IX_ProductMasters_ProductCode",
                table: "ProductMasters");

            migrationBuilder.DropIndex(
                name: "IX_ProductMasters_SerialNo",
                table: "ProductMasters");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNo",
                table: "ProductMasters",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "ProductMasters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProductCode",
                table: "ProductMasters",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProductBarcode",
                table: "ProductMasters",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasters_ProductBarcode",
                table: "ProductMasters",
                column: "ProductBarcode",
                unique: true,
                filter: "[ProductBarcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasters_ProductCode",
                table: "ProductMasters",
                column: "ProductCode",
                unique: true,
                filter: "[ProductCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasters_SerialNo",
                table: "ProductMasters",
                column: "SerialNo",
                unique: true,
                filter: "[SerialNo] IS NOT NULL");
        }
    }
}
