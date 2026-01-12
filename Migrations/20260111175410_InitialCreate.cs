using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blackjack.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerID);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Player1ID = table.Column<int>(type: "int", nullable: false),
                    Player2ID = table.Column<int>(type: "int", nullable: false),
                    StartHP = table.Column<int>(type: "int", nullable: false),
                    Player1HP = table.Column<int>(type: "int", nullable: false),
                    Player2HP = table.Column<int>(type: "int", nullable: false),
                    TurnOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LobbyCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameID);
                    table.ForeignKey(
                        name: "FK_Games_Players_Player1ID",
                        column: x => x.Player1ID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Players_Player2ID",
                        column: x => x.Player2ID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rounds",
                columns: table => new
                {
                    RoundID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameID = table.Column<int>(type: "int", nullable: false),
                    Seed = table.Column<int>(type: "int", nullable: false),
                    Deck = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DamageDealt = table.Column<int>(type: "int", nullable: false),
                    LoserPlayerID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.RoundID);
                    table.ForeignKey(
                        name: "FK_Rounds_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "GameID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerHands",
                columns: table => new
                {
                    PlayerHandID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerID = table.Column<int>(type: "int", nullable: false),
                    RoundID = table.Column<int>(type: "int", nullable: false),
                    DrawnCardsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HandValue = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerHands", x => x.PlayerHandID);
                    table.ForeignKey(
                        name: "FK_PlayerHands_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerHands_Rounds_RoundID",
                        column: x => x.RoundID,
                        principalTable: "Rounds",
                        principalColumn: "RoundID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_Player1ID",
                table: "Games",
                column: "Player1ID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Player2ID",
                table: "Games",
                column: "Player2ID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHands_PlayerID",
                table: "PlayerHands",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHands_RoundID",
                table: "PlayerHands",
                column: "RoundID");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_GameID",
                table: "Rounds",
                column: "GameID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerHands");

            migrationBuilder.DropTable(
                name: "Rounds");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
