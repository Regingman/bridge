using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDataCoinBridge.Migrations
{
    public partial class DataProviderCountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "DataProviders");

            migrationBuilder.CreateTable(
                name: "CountryDataProvider",
                columns: table => new
                {
                    CountriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataProvidersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryDataProvider", x => new { x.CountriesId, x.DataProvidersId });
                    table.ForeignKey(
                        name: "FK_CountryDataProvider_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountryDataProvider_DataProviders_DataProvidersId",
                        column: x => x.DataProvidersId,
                        principalTable: "DataProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CountryDataProvider_DataProvidersId",
                table: "CountryDataProvider",
                column: "DataProvidersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountryDataProvider");

            migrationBuilder.AddColumn<Guid>(
                name: "CountryId",
                table: "DataProviders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
