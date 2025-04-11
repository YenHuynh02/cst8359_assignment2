using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab5.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodDeliveryService",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Fee = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodDeliveryService", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deals",
                columns: table => new
                {
                    FoodDeliveryServiceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    DealTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deals", x => x.FoodDeliveryServiceId);
                    table.ForeignKey(
                        name: "FK_Deals_FoodDeliveryService_FoodDeliveryServiceId",
                        column: x => x.FoodDeliveryServiceId,
                        principalTable: "FoodDeliveryService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    FoodDeliveryServiceId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => new { x.CustomerId, x.FoodDeliveryServiceId });
                    table.ForeignKey(
                        name: "FK_Subscriptions_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_FoodDeliveryService_FoodDeliveryServiceId",
                        column: x => x.FoodDeliveryServiceId,
                        principalTable: "FoodDeliveryService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_FoodDeliveryServiceId",
                table: "Subscriptions",
                column: "FoodDeliveryServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deals");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "FoodDeliveryService");
        }
    }
}
