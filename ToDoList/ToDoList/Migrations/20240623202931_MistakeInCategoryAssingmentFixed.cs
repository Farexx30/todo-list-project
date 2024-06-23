using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.Migrations
{
    /// <inheritdoc />
    public partial class MistakeInCategoryAssingmentFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryAssignments_Assignments_AssignmentId1",
                table: "CategoryAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryAssignments_Categories_CategoryId1",
                table: "CategoryAssignments");

            migrationBuilder.DropIndex(
                name: "IX_CategoryAssignments_AssignmentId1",
                table: "CategoryAssignments");

            migrationBuilder.DropIndex(
                name: "IX_CategoryAssignments_CategoryId1",
                table: "CategoryAssignments");

            migrationBuilder.DropColumn(
                name: "AssignmentId1",
                table: "CategoryAssignments");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "CategoryAssignments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignmentId1",
                table: "CategoryAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId1",
                table: "CategoryAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAssignments_AssignmentId1",
                table: "CategoryAssignments",
                column: "AssignmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAssignments_CategoryId1",
                table: "CategoryAssignments",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryAssignments_Assignments_AssignmentId1",
                table: "CategoryAssignments",
                column: "AssignmentId1",
                principalTable: "Assignments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryAssignments_Categories_CategoryId1",
                table: "CategoryAssignments",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
