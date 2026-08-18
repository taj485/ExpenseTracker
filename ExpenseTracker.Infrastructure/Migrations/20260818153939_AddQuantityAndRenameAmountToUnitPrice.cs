using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityAndRenameAmountToUnitPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Expenses",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Expenses",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Expenses",
                newName: "Amount");
        }
    }
}
