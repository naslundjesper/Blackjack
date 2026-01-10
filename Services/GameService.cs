using Blackjack.Models;

namespace Blackjack.Services
{
    public class GameService
    {
        public Game CreateGame(int player1Id, int player2Id)
        {
            var game = new Game
            {
                Player1ID = player1Id,
                Player2ID = player2Id,
                StartHP = 100,
                Player1HP = 100,
                Player2HP = 100,
                TurnOrder = player1Id,
                Status = "InProgress"
            };

            return game;
        }
    }
}
