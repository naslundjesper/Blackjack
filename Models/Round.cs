using System.Collections.Generic;

namespace Blackjack.Models
{
    public class Round
    {
        //Primärnyckel för rundan
        public int RoundID { get; set; }
        //Vilket game rundan tillhör
        public int GameID { get; set; }
        //Generera kortlek
        public int Seed { get; set; }
        //Kortlek (bitvector), 7 bytes för 52 kort
        //Idé: 0 = kortet är kvar i leken, 1 = kortet är draget
        public byte[] Deck { get; set; } = new byte[7];
        //Skadan som delas ut i slutet av rundan
        public int DamageDealt { get; set; }
        public int LoserPlayerID { get; set; }

        //Referens till spelet som rundan tillhör
        public Game Game { get; set; } = null!;

        public ICollection<PlayerHand> PlayerHands { get; set; } = new List<PlayerHand>();

    }
}
