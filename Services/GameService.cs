using Blackjack.Models;

namespace Blackjack.Services
{
    public class GameService
    {
        public Game CreateGame(int player1Id, int? player2Id = null)
        {
            return new Game
            {
                Player1ID = player1Id,
                Player2ID = player2Id, // Nu som null
                StartHP = 100,
                Player1HP = 100,
                Player2HP = 100,
                TurnOrder = player1Id,
                Status = "Waiting",
                LobbyCode = "TEMP"
            };
        }
    }
}