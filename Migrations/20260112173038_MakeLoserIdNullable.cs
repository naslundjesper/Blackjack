using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blackjack.Migrations
{
    /// <inheritdoc />
    public partial class MakeLoserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Seed",
                table: "Rounds");

            migrationBuilder.AlterColumn<int>(
                name: "LoserPlayerID",
                table: "Rounds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LoserPlayerID",
                table: "Rounds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Seed",
                table: "Rounds",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
