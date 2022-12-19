using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDataCoinBridge.Migrations
{
    public partial class updateDatabaseTransactionStatistic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Email",
                table: "UserTermsOfUses",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Phone",
                table: "UserTermsOfUses",
                type: "text[]",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BridgeTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    ProviderId = table.Column<string>(type: "text", nullable: true),
                    ProviderName = table.Column<string>(type: "text", nullable: true),
                    Count = table.Column<decimal>(type: "numeric", nullable: false),
                    USDMDC = table.Column<decimal>(type: "numeric", nullable: false),
                    Commission = table.Column<decimal>(type: "numeric", nullable: false),
                    RewardCategoryId = table.Column<string>(type: "text", nullable: true),
                    RewardCategoryName = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BridgeTransactions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BridgeTransactions");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserTermsOfUses");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "UserTermsOfUses");
        }
    }
}
