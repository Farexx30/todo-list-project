using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeletingAssignmentsWhenCategoryDeletedAndLongerCategoryName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);
        }
    }
}
