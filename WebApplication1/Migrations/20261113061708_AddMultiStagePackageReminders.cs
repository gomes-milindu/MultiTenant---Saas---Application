using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiStagePackageReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExpirationEmailSent",
                table: "TeacherPackages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FirstReminderSent",
                table: "TeacherPackages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SecondReminderSent",
                table: "TeacherPackages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PackageReminderSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EnableFirstReminder = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FirstReminderDaysBefore = table.Column<int>(type: "int", nullable: false),
                    EnableSecondReminder = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SecondReminderDaysBefore = table.Column<int>(type: "int", nullable: false),
                    EnableExpirationEmail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TargetScope = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageReminderSettings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PackageReminderTargetTeachers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TeacherUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageReminderTargetTeachers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageReminderSettings");

            migrationBuilder.DropTable(
                name: "PackageReminderTargetTeachers");

            migrationBuilder.DropColumn(
                name: "ExpirationEmailSent",
                table: "TeacherPackages");

            migrationBuilder.DropColumn(
                name: "FirstReminderSent",
                table: "TeacherPackages");

            migrationBuilder.DropColumn(
                name: "SecondReminderSent",
                table: "TeacherPackages");
        }
    }
}
