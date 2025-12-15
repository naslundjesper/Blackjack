using System.Collections.Generic;

namespace Blackjack.Models
{
    public class PlayerHand
    {
        // Primärnyckel
        public int PlayerHandID { get; set; }

        // Vilken spelare handen tillhör
        public int PlayerID { get; set; }
        public Player Player { get; set; } = null!;

        // Vilken runda handen tillhör
        public int RoundID { get; set; }
        public Round Round { get; set; } = null!;

        // Refererar till kort i Round.Deck
        public List<int> DrawnCards { get; set; } = new List<int>();

        // Handens totala värde
        public int HandValue { get; set; }

        // Om handen är avslutad (stand eller bust)
        public bool IsCompleted { get; set; }
    }
}
