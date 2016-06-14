using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shevastream");

            migrationBuilder.CreateTable(
                name: "Customer",
                schema: "shevastream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    Email = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SSFeedback",
                schema: "shevastream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    Body = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Timestamp = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SSFeedback", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogEntry",
                schema: "shevastream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    TargetId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<long>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                schema: "shevastream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                schema: "shevastream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                schema: "shevastream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    Characteristics = table.Column<string>(nullable: true),
                    Cost = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ImageUrls = table.Column<string>(nullable: true),
                    Information = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    VideoData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentMethod",
                schema: "shevastream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    Cost = table.Column<int>(nullable: false),
                    CostTBD = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "shevastream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    FullName = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    PassHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogPost",
                schema: "shevastream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    Active = table.Column<bool>(nullable: false),
                    AuthorId = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    DatePosted = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    TitleUrl = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPost", x => x.Id);
                    table.UniqueConstraint("AK_BlogPost_Title", x => x.Title);
                    table.UniqueConstraint("AK_BlogPost_TitleUrl", x => x.TitleUrl);
                    table.ForeignKey(
                        name: "FK_BlogPost_User_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "shevastream",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                schema: "shevastream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    Address = table.Column<string>(nullable: true),
                    AssigneeComment = table.Column<string>(nullable: true),
                    AssigneeId = table.Column<int>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    CustomerId = table.Column<int>(nullable: false),
                    DateCreated = table.Column<long>(nullable: false),
                    DateLastModified = table.Column<long>(nullable: false),
                    OrderStatusId = table.Column<int>(nullable: false),
                    PaymentMethodId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    ShipmentMethodId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_User_AssigneeId",
                        column: x => x.AssigneeId,
                        principalSchema: "shevastream",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "shevastream",
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_OrderStatus_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalSchema: "shevastream",
                        principalTable: "OrderStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "shevastream",
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "shevastream",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_ShipmentMethod_ShipmentMethodId",
                        column: x => x.ShipmentMethodId,
                        principalSchema: "shevastream",
                        principalTable: "ShipmentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "OrderProduct",
                schema: "shevastream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    OrderId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderProduct_Order_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "shevastream",
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "shevastream",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPost_AuthorId",
                schema: "shevastream",
                table: "BlogPost",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_AssigneeId",
                schema: "shevastream",
                table: "Order",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerId",
                schema: "shevastream",
                table: "Order",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderStatusId",
                schema: "shevastream",
                table: "Order",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PaymentMethodId",
                schema: "shevastream",
                table: "Order",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ProductId",
                schema: "shevastream",
                table: "Order",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ShipmentMethodId",
                schema: "shevastream",
                table: "Order",
                column: "ShipmentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_OrderId",
                schema: "shevastream",
                table: "OrderProduct",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_ProductId",
                schema: "shevastream",
                table: "OrderProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PushPair_UserId",
                schema: "shevastream",
                table: "PushPair",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPost",
                schema: "shevastream");

            migrationBuilder.DropTable(
                name: "SSFeedback",
                schema: "shevastream");

            migrationBuilder.DropTable(
                name: "LogEntry",
                schema: "shevastream");

            migrationBuilder.DropTable(
                name: "OrderProduct",
                schema: "shevastream");

            migrationBuilder.DropTable(
                name: "PushPair",
                schema: "shevastream");

            migrationBuilder.DropTable(
                name: "Order",
                schema: "shevastream");

            migrationBuilder.DropTable(
                name: "User",
                schema: "shevastream");

            migrationBuilder.DropTable(
                name: "Customer",
                schema: "shevastream");

            migrationBuilder.DropTable(
                name: "OrderStatus",
                schema: "shevastream");

            migrationBuilder.DropTable(
                name: "PaymentMethod",
                schema: "shevastream");

            migrationBuilder.DropTable(
                name: "Product",
                schema: "shevastream");

            migrationBuilder.DropTable(
                name: "ShipmentMethod",
                schema: "shevastream");
        }
    }
}
