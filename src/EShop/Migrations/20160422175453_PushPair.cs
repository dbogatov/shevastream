using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace EShop.Migrations
{
    public partial class PushPair : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Order_Customer_CustomerId", schema: "shevastream", table: "Order");
            migrationBuilder.DropForeignKey(name: "FK_Order_OrderStatus_OrderStatusId", schema: "shevastream", table: "Order");
            migrationBuilder.DropForeignKey(name: "FK_Order_PaymentMethod_PaymentMethodId", schema: "shevastream", table: "Order");
            migrationBuilder.DropForeignKey(name: "FK_Order_Product_ProductId", schema: "shevastream", table: "Order");
            migrationBuilder.DropForeignKey(name: "FK_Order_ShipmentMethod_ShipmentMethodId", schema: "shevastream", table: "Order");
            migrationBuilder.CreateTable(
                name: "PushPair",
                schema: "shevastream",
                columns: table => new
                {
                    DeviceToken = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushPair", x => x.DeviceToken);
                    table.ForeignKey(
                        name: "FK_PushPair_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "shevastream",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "shevastream",
                table: "User",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_Order_Customer_CustomerId",
                schema: "shevastream",
                table: "Order",
                column: "CustomerId",
                principalSchema: "shevastream",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Order_OrderStatus_OrderStatusId",
                schema: "shevastream",
                table: "Order",
                column: "OrderStatusId",
                principalSchema: "shevastream",
                principalTable: "OrderStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Order_PaymentMethod_PaymentMethodId",
                schema: "shevastream",
                table: "Order",
                column: "PaymentMethodId",
                principalSchema: "shevastream",
                principalTable: "PaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Order_Product_ProductId",
                schema: "shevastream",
                table: "Order",
                column: "ProductId",
                principalSchema: "shevastream",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Order_ShipmentMethod_ShipmentMethodId",
                schema: "shevastream",
                table: "Order",
                column: "ShipmentMethodId",
                principalSchema: "shevastream",
                principalTable: "ShipmentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Order_Customer_CustomerId", schema: "shevastream", table: "Order");
            migrationBuilder.DropForeignKey(name: "FK_Order_OrderStatus_OrderStatusId", schema: "shevastream", table: "Order");
            migrationBuilder.DropForeignKey(name: "FK_Order_PaymentMethod_PaymentMethodId", schema: "shevastream", table: "Order");
            migrationBuilder.DropForeignKey(name: "FK_Order_Product_ProductId", schema: "shevastream", table: "Order");
            migrationBuilder.DropForeignKey(name: "FK_Order_ShipmentMethod_ShipmentMethodId", schema: "shevastream", table: "Order");
            migrationBuilder.DropColumn(name: "ImageUrl", schema: "shevastream", table: "User");
            migrationBuilder.DropTable(name: "PushPair", schema: "shevastream");
            migrationBuilder.AddForeignKey(
                name: "FK_Order_Customer_CustomerId",
                schema: "shevastream",
                table: "Order",
                column: "CustomerId",
                principalSchema: "shevastream",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Order_OrderStatus_OrderStatusId",
                schema: "shevastream",
                table: "Order",
                column: "OrderStatusId",
                principalSchema: "shevastream",
                principalTable: "OrderStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Order_PaymentMethod_PaymentMethodId",
                schema: "shevastream",
                table: "Order",
                column: "PaymentMethodId",
                principalSchema: "shevastream",
                principalTable: "PaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Order_Product_ProductId",
                schema: "shevastream",
                table: "Order",
                column: "ProductId",
                principalSchema: "shevastream",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Order_ShipmentMethod_ShipmentMethodId",
                schema: "shevastream",
                table: "Order",
                column: "ShipmentMethodId",
                principalSchema: "shevastream",
                principalTable: "ShipmentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
