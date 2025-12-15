namespace Blackjack.Models
{
    public class Game
    {
        // Unikt ID för varje spel
        public int GameID { get; set; }

        public int Player1ID { get; set; }
        public int Player2ID { get; set; }

        public Player Player1 { get; set; } = null!;
        public Player Player2 { get; set; } = null!;

        //Startvärde för HP
        public int StartHP { get; set; }
        //Anger vilken spelares tur det är
        public int TurnOrder { get; set; }
        //Waiting, finished ex. 
        public string Status { get; set; }
        //Kod för att gå med i lobby
        public string LobbyCode { get; set; } = null!;
       
        //Innehåller alla rundor i spelet
        public ICollection<Round> Rounds { get; set; } = new List<Round>();

    }
}
