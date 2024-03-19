using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transaction.API.Migrations
{
    /// <inheritdoc />
    public partial class AmountTransactionChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewAccountBalance",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OldAccountBalance",
                table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "NewAccountBalance",
                table: "Transactions",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OldAccountBalance",
                table: "Transactions",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
