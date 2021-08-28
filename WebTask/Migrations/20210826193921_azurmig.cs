using Microsoft.EntityFrameworkCore.Migrations;

namespace WebTask.Migrations
{
    public partial class azurmig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_collects_CollectId",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_items_CollectId",
                table: "items");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_items_CollectId",
                table: "items",
                column: "CollectId");

            migrationBuilder.AddForeignKey(
                name: "FK_items_collects_CollectId",
                table: "items",
                column: "CollectId",
                principalTable: "collects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
