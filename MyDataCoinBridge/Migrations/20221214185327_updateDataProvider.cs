using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDataCoinBridge.Migrations
{
    public partial class updateDataProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BridgeUserId",
                table: "DataProviders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataProviders_BridgeUserId",
                table: "DataProviders",
                column: "BridgeUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataProviders_BridgeUsers_BridgeUserId",
                table: "DataProviders",
                column: "BridgeUserId",
                principalTable: "BridgeUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataProviders_BridgeUsers_BridgeUserId",
                table: "DataProviders");

            migrationBuilder.DropIndex(
                name: "IX_DataProviders_BridgeUserId",
                table: "DataProviders");

            migrationBuilder.DropColumn(
                name: "BridgeUserId",
                table: "DataProviders");
        }
    }
}
