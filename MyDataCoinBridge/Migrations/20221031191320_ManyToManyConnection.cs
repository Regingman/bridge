using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDataCoinBridge.Migrations
{
    public partial class ManyToManyConnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardCategories_DataProviders_DataProviderId",
                table: "RewardCategories");

            migrationBuilder.DropIndex(
                name: "IX_RewardCategories_DataProviderId",
                table: "RewardCategories");

            migrationBuilder.DropColumn(
                name: "DataProviderId",
                table: "RewardCategories");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RewardCategories",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DataProviderRewardCategory",
                columns: table => new
                {
                    DataProvidersId = table.Column<Guid>(type: "uuid", nullable: false),
                    RewardCategoriesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProviderRewardCategory", x => new { x.DataProvidersId, x.RewardCategoriesId });
                    table.ForeignKey(
                        name: "FK_DataProviderRewardCategory_DataProviders_DataProvidersId",
                        column: x => x.DataProvidersId,
                        principalTable: "DataProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataProviderRewardCategory_RewardCategories_RewardCategorie~",
                        column: x => x.RewardCategoriesId,
                        principalTable: "RewardCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataProviderRewardCategory_RewardCategoriesId",
                table: "DataProviderRewardCategory",
                column: "RewardCategoriesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataProviderRewardCategory");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RewardCategories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "DataProviderId",
                table: "RewardCategories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RewardCategories_DataProviderId",
                table: "RewardCategories",
                column: "DataProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_RewardCategories_DataProviders_DataProviderId",
                table: "RewardCategories",
                column: "DataProviderId",
                principalTable: "DataProviders",
                principalColumn: "Id");
        }
    }
}
