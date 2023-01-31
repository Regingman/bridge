using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDataCoinBridge.Migrations
{
    public partial class WebHook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebHooks",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    WebHookUrl = table.Column<string>(type: "text", nullable: true),
                    Secret = table.Column<string>(type: "text", nullable: true),
                    ContentType = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    HookEvents = table.Column<int[]>(type: "integer[]", nullable: true),
                    LastTrigger = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHooks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WebHookHeader",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    WebHookID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHookHeader", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WebHookHeader_WebHooks_WebHookID",
                        column: x => x.WebHookID,
                        principalTable: "WebHooks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebHooksHistory",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    WebHookID = table.Column<Guid>(type: "uuid", nullable: false),
                    Guid = table.Column<string>(type: "text", nullable: true),
                    Result = table.Column<int>(type: "integer", nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    RequestBody = table.Column<string>(type: "text", nullable: true),
                    RequestHeaders = table.Column<string>(type: "text", nullable: true),
                    Exception = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHooksHistory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WebHooksHistory_WebHooks_WebHookID",
                        column: x => x.WebHookID,
                        principalTable: "WebHooks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebHookHeader_WebHookID",
                table: "WebHookHeader",
                column: "WebHookID");

            migrationBuilder.CreateIndex(
                name: "IX_WebHooksHistory_WebHookID",
                table: "WebHooksHistory",
                column: "WebHookID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebHookHeader");

            migrationBuilder.DropTable(
                name: "WebHooksHistory");

            migrationBuilder.DropTable(
                name: "WebHooks");
        }
    }
}
