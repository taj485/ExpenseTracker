using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseCreatedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Expenses",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CreatedByUserId",
                table: "Expenses",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Users_CreatedByUserId",
                table: "Expenses",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Users_CreatedByUserId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_CreatedByUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Expenses");
        }
    }
}
