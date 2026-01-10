using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blackjack.Migrations
{
    /// <inheritdoc />
    public partial class AddHPToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Player1HP",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player2HP",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1HP",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Player2HP",
                table: "Games");
        }
    }
}
