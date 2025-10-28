using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TooLiRent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    CatalogNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tools_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsReturned = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rentals_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RentalDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalId = table.Column<int>(type: "int", nullable: false),
                    ToolId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalDetails_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalDetails_Tools_ToolId",
                        column: x => x.ToolId,
                        principalTable: "Tools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tools for drilling", "Drilling", new DateTime(2025, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tools for cutting", "Cutting", new DateTime(2025, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tools for grinding", "Grinding", new DateTime(2025, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tools for sanding", "Sanding", new DateTime(2025, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tools for Detailing", "Detailing", new DateTime(2025, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Air Compressor tools", "Air Tools", new DateTime(2025, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tools for painting", "Painting", new DateTime(2025, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PhoneNumber", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "anna@example.com", "Anna Andersson", "0701234561", 1, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2024, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "bjorn@example.com", "Björn Berg", "0701234562", 1, new DateTime(2024, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2024, 5, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "cecilia@example.com", "Cecilia Carlsson", "0701234563", 1, new DateTime(2024, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2024, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "david@example.com", "David Dahl", "0701234564", 1, new DateTime(2024, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2024, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "eva@example.com", "Eva Ek", "0701234565", 1, new DateTime(2024, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2024, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "filip@example.com", "Filip Fors", "0701234566", 1, new DateTime(2024, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2024, 5, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "greta@example.com", "Greta Gustavsson", "0701234567", 1, new DateTime(2024, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, new DateTime(2024, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "henrik@example.com", "Henrik Hall", "0701234568", 1, new DateTime(2024, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, new DateTime(2024, 5, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "ida@example.com", "Ida Isaksson", "0701234569", 1, new DateTime(2024, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, new DateTime(2024, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "johan@example.com", "Johan Jönsson", "0701234570", 1, new DateTime(2024, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Rentals",
                columns: new[] { "Id", "CreatedAt", "CustomerId", "EndDate", "IsReturned", "StartDate", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2025, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2025, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2025, 9, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2025, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2025, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2025, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2025, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2025, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, new DateTime(2025, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2025, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2025, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, new DateTime(2025, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2025, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Tools",
                columns: new[] { "Id", "CatalogNumber", "CategoryId", "CreatedAt", "Description", "Name", "Price", "Status", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "DRL-1001", 1, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A powerful cordless drill for all your DIY needs.", "Cordless Drill", 25, 1, 10, new DateTime(2025, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "SAW-2002", 2, new DateTime(2025, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "High-precision circular saw for wood and metal.", "Circular Saw", 30, 1, 5, new DateTime(2025, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "HDR-3003", 1, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Heavy-duty hammer drill for concrete and masonry.", "Hammer Drill", 28, 1, 7, new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "AGR-4004", 3, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Versatile angle grinder for cutting and grinding.", "Angle Grinder", 22, 1, 8, new DateTime(2025, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "JSG-5005", 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Compact jigsaw for intricate cutting tasks.", "Jigsaw", 18, 1, 6, new DateTime(2025, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "SND-6006", 4, new DateTime(2024, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Electric sander for smooth finishing.", "Power Sander", 20, 1, 9, new DateTime(2024, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "RTY-7007", 5, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Multi-purpose rotary tool for detailed work.", "Rotary Tool", 15, 1, 12, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "TBS-8008", 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Large table saw for professional carpentry.", "Table Saw", 40, 1, 3, new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "AIR-9009", 6, new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Portable air compressor for pneumatic tools.", "Air Compressor", 35, 1, 4, new DateTime(2024, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, "PNT-1010", 7, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Efficient paint sprayer for large surfaces.", "Paint Sprayer", 27, 1, 5, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, "AIR-9010", 6, new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Portable air compressor for pneumatic tools.", "Air Compressor", 35, 2, 1, new DateTime(2024, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "RentalDetails",
                columns: new[] { "Id", "CreatedAt", "Quantity", "RentalId", "ToolId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, 1, new DateTime(2024, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 3, new DateTime(2024, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2024, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2024, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 3, 5, new DateTime(2024, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2024, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, 4, new DateTime(2024, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RentalDetails_RentalId",
                table: "RentalDetails",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalDetails_ToolId",
                table: "RentalDetails",
                column: "ToolId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CustomerId",
                table: "Rentals",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tools_CategoryId",
                table: "Tools",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RentalDetails");

            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "Tools");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
