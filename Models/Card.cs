namespace Blackjack.Models
{
    public class Card
    {
        public int CardIndex { get; set; }

        public int Value
        {
            get
            {
                int rank = (CardIndex % 13) + 1;
                if (rank == 1) return 11; // Ess
                if (rank > 10) return 10; // K, Q, J
                return rank;
            }
        }
        public string DisplayName => $"{Suit} {RankName}";

        public string RankName => ((CardIndex % 13) + 1) switch
        {
            1 => "Ess",
            11 => "Knekt",
            12 => "Dam",
            13 => "Kung",
            var r => r.ToString()
        };

        public string Suit => (CardIndex / 13) switch
        {
            0 => "Hjärter",
            1 => "Spader",
            2 => "Ruter",
            _ => "Klöver"
        };
    }
}