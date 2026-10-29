using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerTeacherToBatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Batches_InstituteId_Name",
                table: "Batches");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerTeacherId",
                table: "Batches",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_InstituteId",
                table: "Batches",
                column: "InstituteId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_InstituteId_OwnerTeacherId_Name",
                table: "Batches",
                columns: new[] { "InstituteId", "OwnerTeacherId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Batches_OwnerTeacherId",
                table: "Batches",
                column: "OwnerTeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Users_OwnerTeacherId",
                table: "Batches",
                column: "OwnerTeacherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Users_OwnerTeacherId",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_InstituteId",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_InstituteId_OwnerTeacherId_Name",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_OwnerTeacherId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "OwnerTeacherId",
                table: "Batches");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_InstituteId_Name",
                table: "Batches",
                columns: new[] { "InstituteId", "Name" },
                unique: true);
        }
    }
}
