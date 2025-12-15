using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;


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

        // Lagring i databasen (JSON-text)
        public string DrawnCardsJson { get; set; } = "[]";

        //Används i koden, inte databasen
        [NotMapped]
        public List<int> DrawnCards
        {
            get => JsonSerializer.Deserialize<List<int>>(DrawnCardsJson)!;
            set => DrawnCardsJson = JsonSerializer.Serialize(value);
        }


        // Handens totala värde
        public int HandValue { get; set; }

        // Om handen är avslutad (stand eller bust)
        public bool IsCompleted { get; set; }
    }
}
