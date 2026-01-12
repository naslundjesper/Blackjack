using Blackjack.Models;

namespace Blackjack.Services
{
    public class GameService
    {
        
        public Game CreateNewGame(int p1Id, string lobbyCode)
        {
            return new Game
            {
                Player1ID = p1Id,
                LobbyCode = lobbyCode.ToUpper(),

                // Startvärden för spelet
                StartHP = 100,
                Player1HP = 100,
                Player2HP = 100,

                // Status och Turordning
                Status = "Waiting",
                TurnOrder = p1Id // Spelare 1 börjar alltid
            };
        }
    }
}