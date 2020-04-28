using Microsoft.EntityFrameworkCore.Migrations;

namespace SportsStore.Migrations
{
    public partial class AddImportOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportedProduct_Product_ProductID",
                table: "ImportedProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImportedProduct",
                table: "ImportedProduct");

            migrationBuilder.DropIndex(
                name: "IX_ImportedProduct_ImportOrderID",
                table: "ImportedProduct");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "ImportedProduct");

            migrationBuilder.AddColumn<string>(
                name: "WholesalerAddress",
                table: "ImportOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WholesalerName",
                table: "ImportOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WholesalerPhone",
                table: "ImportOrder",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImportedProduct",
                table: "ImportedProduct",
                columns: new[] { "ImportOrderID", "ProductID" });

            migrationBuilder.AddForeignKey(
                name: "FK_ImportedProduct_Product_ProductID",
                table: "ImportedProduct",
                column: "ProductID",
                principalTable: "Product",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportedProduct_Product_ProductID",
                table: "ImportedProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImportedProduct",
                table: "ImportedProduct");

            migrationBuilder.DropColumn(
                name: "WholesalerAddress",
                table: "ImportOrder");

            migrationBuilder.DropColumn(
                name: "WholesalerName",
                table: "ImportOrder");

            migrationBuilder.DropColumn(
                name: "WholesalerPhone",
                table: "ImportOrder");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "ImportedProduct",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImportedProduct",
                table: "ImportedProduct",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_ImportedProduct_ImportOrderID",
                table: "ImportedProduct",
                column: "ImportOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportedProduct_Product_ProductID",
                table: "ImportedProduct",
                column: "ProductID",
                principalTable: "Product",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
