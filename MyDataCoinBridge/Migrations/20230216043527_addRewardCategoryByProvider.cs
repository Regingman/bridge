using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDataCoinBridge.Migrations
{
    /// <inheritdoc />
    public partial class addRewardCategoryByProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RewardCategoryByProviders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RewardCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardCategoryByProviders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RewardCategoryByProviders_DataProviders_DataProviderId",
                        column: x => x.DataProviderId,
                        principalTable: "DataProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RewardCategoryByProviders_RewardCategories_RewardCategoryId",
                        column: x => x.RewardCategoryId,
                        principalTable: "RewardCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RewardCategoryByProviders_DataProviderId",
                table: "RewardCategoryByProviders",
                column: "DataProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardCategoryByProviders_RewardCategoryId",
                table: "RewardCategoryByProviders",
                column: "RewardCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RewardCategoryByProviders");
        }
    }
}
