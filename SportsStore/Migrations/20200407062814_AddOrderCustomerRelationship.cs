using Microsoft.EntityFrameworkCore.Migrations;

namespace SportsStore.Migrations
{
    public partial class AddOrderCustomerRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_CustomerID",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "RecipientAddress",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientName",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientPhone",
                table: "Order",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AspNetUsers_CustomerID",
                table: "Order",
                column: "CustomerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_CustomerID",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RecipientAddress",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RecipientName",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RecipientPhone",
                table: "Order");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AspNetUsers_CustomerID",
                table: "Order",
                column: "CustomerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
