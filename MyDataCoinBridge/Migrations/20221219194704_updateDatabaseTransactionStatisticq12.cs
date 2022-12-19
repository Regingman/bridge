using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDataCoinBridge.Migrations
{
    public partial class updateDatabaseTransactionStatisticq12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Claim",
                table: "BridgeTransactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Claim",
                table: "BridgeTransactions");
        }
    }
}
