namespace Blackjack.Services
{
    public class CardService
    {
        public List<int> CreateDeck()
        {
            var deck = new List<int>();
            for (int i = 0; i < 52; i++)
                deck.Add(i);

            return deck;
        }

        public void Shuffle(List<int> deck, int seed)
        {
            var rng = new Random(seed);

            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (deck[i], deck[j]) = (deck[j], deck[i]);
            }
        }

        public int DrawCard(List<int> deck)
        {
            if (deck.Count == 0)
                throw new InvalidOperationException("Deck is empty");

            int card = deck[0];
            deck.RemoveAt(0);
            return card;
        }
    }
}
