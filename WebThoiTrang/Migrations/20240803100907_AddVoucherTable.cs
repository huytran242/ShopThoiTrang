using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebThoiTrang.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "vochers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_vochers_OrderId",
                table: "vochers",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_vochers_orders_OrderId",
                table: "vochers",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vochers_orders_OrderId",
                table: "vochers");

            migrationBuilder.DropIndex(
                name: "IX_vochers_OrderId",
                table: "vochers");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "vochers");
        }
    }
}
