using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ToDoList.Migrations
{
    /// <inheritdoc />
    public partial class DbStructureUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryAssignments",
                table: "CategoryAssignments");

            migrationBuilder.DropIndex(
                name: "IX_CategoryAssignments_CategoryId",
                table: "CategoryAssignments");

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "Assignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsChecked",
                table: "Assignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsImportant",
                table: "Assignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryAssignments",
                table: "CategoryAssignments",
                columns: new[] { "CategoryId", "AssignmentId" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "IsBuiltIn", "Name", "UserId" },
                values: new object[,]
                {
                    { 1, true, "Dom", null },
                    { 2, true, "Praca", null },
                    { 3, true, "Edukacja", null },
                    { 4, true, "Zadania", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAssignments_AssignmentId",
                table: "CategoryAssignments",
                column: "AssignmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryAssignments",
                table: "CategoryAssignments");

            migrationBuilder.DropIndex(
                name: "IX_CategoryAssignments_AssignmentId",
                table: "CategoryAssignments");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "IsChecked",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "IsImportant",
                table: "Assignments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryAssignments",
                table: "CategoryAssignments",
                columns: new[] { "AssignmentId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAssignments_CategoryId",
                table: "CategoryAssignments",
                column: "CategoryId");
        }
    }
}
