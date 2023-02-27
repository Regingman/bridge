using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDataCoinBridge.Migrations
{
    /// <inheritdoc />
    public partial class UserPrivacySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPrivacySettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Profile = table.Column<bool>(type: "boolean", nullable: false),
                    BasicData = table.Column<bool>(type: "boolean", nullable: false),
                    Contacts = table.Column<bool>(type: "boolean", nullable: false),
                    WorkAndEducation = table.Column<bool>(type: "boolean", nullable: false),
                    PlaceOfResidence = table.Column<bool>(type: "boolean", nullable: false),
                    PersonalInterests = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrivacySettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPrivacySettings");
        }
    }
}
