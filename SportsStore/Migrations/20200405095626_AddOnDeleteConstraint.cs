using Microsoft.EntityFrameworkCore.Migrations;

namespace SportsStore.Migrations
{
    public partial class AddOnDeleteConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedProduct_Product_ProductID",
                table: "OrderedProduct");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedProduct_Product_ProductID",
                table: "OrderedProduct",
                column: "ProductID",
                principalTable: "Product",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedProduct_Product_ProductID",
                table: "OrderedProduct");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedProduct_Product_ProductID",
                table: "OrderedProduct",
                column: "ProductID",
                principalTable: "Product",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
