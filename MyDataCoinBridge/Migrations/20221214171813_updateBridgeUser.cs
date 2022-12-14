using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDataCoinBridge.Migrations
{
    public partial class updateBridgeUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "BridgeUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TokenForService",
                table: "BridgeUsers",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "BridgeUsers");

            migrationBuilder.DropColumn(
                name: "TokenForService",
                table: "BridgeUsers");
        }
    }
}
