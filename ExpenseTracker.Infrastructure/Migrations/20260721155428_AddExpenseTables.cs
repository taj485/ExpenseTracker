using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExpenseTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseUsers");

            migrationBuilder.AddColumn<int>(
                name: "ExpenseTableId",
                table: "Expenses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExpenseTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseTables_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserExpenseTables",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExpenseTableId = table.Column<int>(type: "integer", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExpenseTables", x => new { x.UserId, x.ExpenseTableId });
                    table.ForeignKey(
                        name: "FK_UserExpenseTables_ExpenseTables_ExpenseTableId",
                        column: x => x.ExpenseTableId,
                        principalTable: "ExpenseTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserExpenseTables_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseTableId",
                table: "Expenses",
                column: "ExpenseTableId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTables_CreatedByUserId",
                table: "ExpenseTables",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExpenseTables_ExpenseTableId",
                table: "UserExpenseTables",
                column: "ExpenseTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseTables_ExpenseTableId",
                table: "Expenses",
                column: "ExpenseTableId",
                principalTable: "ExpenseTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseTables_ExpenseTableId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "UserExpenseTables");

            migrationBuilder.DropTable(
                name: "ExpenseTables");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseTableId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ExpenseTableId",
                table: "Expenses");

            migrationBuilder.CreateTable(
                name: "ExpenseUsers",
                columns: table => new
                {
                    ExpensesId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseUsers", x => new { x.ExpensesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ExpenseUsers_Expenses_ExpensesId",
                        column: x => x.ExpensesId,
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseUsers_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseUsers_UsersId",
                table: "ExpenseUsers",
                column: "UsersId");
        }
    }
}
