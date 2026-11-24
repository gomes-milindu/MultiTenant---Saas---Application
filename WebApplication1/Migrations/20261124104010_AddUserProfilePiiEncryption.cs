using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfilePiiEncryption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailHash",
                table: "UserProfiles",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdHash",
                table: "UserProfiles",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumberHash",
                table: "UserProfiles",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_EmailHash",
                table: "UserProfiles",
                column: "EmailHash");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_NationalIdHash",
                table: "UserProfiles",
                column: "NationalIdHash");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_PhoneNumberHash",
                table: "UserProfiles",
                column: "PhoneNumberHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_EmailHash",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_NationalIdHash",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_PhoneNumberHash",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "EmailHash",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "NationalIdHash",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "PhoneNumberHash",
                table: "UserProfiles");
        }
    }
}
