using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TooLiRent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTool12Status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Tools",
                keyColumn: "Id",
                keyValue: 12,
                column: "Status",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Tools",
                keyColumn: "Id",
                keyValue: 12,
                column: "Status",
                value: 2);
        }
    }
}
