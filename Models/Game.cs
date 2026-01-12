using System.Collections.Generic;

namespace Blackjack.Models
{
    public class Game
    {
        public int GameID { get; set; }

        public int Player1ID { get; set; }
        public int? Player2ID { get; set; } // int? tillåter null

        public Player Player1 { get; set; } = null!;
        public Player? Player2 { get; set; }

        public int StartHP { get; set; }
        public int Player1HP { get; set; }
        public int Player2HP { get; set; }
        public int TurnOrder { get; set; }
        public string Status { get; set; } = "Waiting";
        public string LobbyCode { get; set; } = null!;

        public ICollection<Round> Rounds { get; set; } = new List<Round>();
    }
}