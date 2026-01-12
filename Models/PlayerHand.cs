using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Blackjack.Models
{
    public class PlayerHand
    {
        public int PlayerHandID { get; set; }
        public int PlayerID { get; set; }
        public int RoundID { get; set; }
        public string DrawnCardsJson { get; set; } = "[]";
        public int HandValue { get; set; }
        public bool IsCompleted { get; set; }

        [NotMapped]
        public List<int> DrawnCards
        {
            get => JsonSerializer.Deserialize<List<int>>(DrawnCardsJson) ?? new List<int>();
            set => DrawnCardsJson = JsonSerializer.Serialize(value);
        }
    }
}