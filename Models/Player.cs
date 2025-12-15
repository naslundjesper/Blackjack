using System.Collections.Generic;

namespace Blackjack.Models
{
    public class Player
    {
        // Unikt ID för varje spelare
        public int PlayerID { get; set; }

        // Login information
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        // Spel där spelaren är Player1
        public ICollection<Game> GamesAsPlayer1 { get; set; } = new List<Game>();

        // Spel där spelaren är Player2
        public ICollection<Game> GamesAsPlayer2 { get; set; } = new List<Game>();

        // Alla händer som tillhör spelaren
        public ICollection<PlayerHand> PlayerHands { get; set; } = new List<PlayerHand>();


    }
}
