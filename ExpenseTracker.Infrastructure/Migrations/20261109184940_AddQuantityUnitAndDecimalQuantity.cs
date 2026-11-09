using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityUnitAndDecimalQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "Expenses",
                type: "numeric(10,3)",
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Expenses",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "Item");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Expenses");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "Expenses",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,3)",
                oldDefaultValue: 1m);
        }
    }
}
