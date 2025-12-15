using System.Collections.Generic;
namespace Blackjack.Models
{
    public class PlayerHand
    {
        //Primärnyckel för PlayerHand
        public int PlayerHandID { get; set; }
        //Vilken spelare handen tillhör
        public int PlayerID { get; set; }
        //Vilken runda handen tillhör
        public int RoundID { get; set; }
        //Refererar till ett kort i Round.Deck
        public List<int> DrawnCards { get; set; } = new List<int>();
        //Handens totala värde
        public int HandValue { get; set; }
        //Om handen är avslutad (stand eller bust)
        public bool IsCompleted { get; set; }
        //Referens till spelaren
        public Player Player { get; set; } = null!;
        //Referens till rundan
        public Round Round { get; set; } = null!;

    }
}
