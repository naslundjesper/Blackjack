namespace Blackjack.Services
{
    public class BlackjackRuleService
    {
        public int GetCardValue(int card)
        {
            int rank = card % 13;

            if (rank == 0) return 11;        // Ace
            if (rank >= 10) return 10;       // J, Q, K
            return rank + 1;                // 2–10
        }

        //Räkna värdet av en hand med kort
        public int CalculateHandValue(List<int> cards)
        {
            int total = 0;
            int aces = 0;

            foreach (var card in cards)
            {
                int value = GetCardValue(card);
                total += value;
                if (value == 11) aces++;
            }

            while (total > 21 && aces > 0)
            {
                total -= 10;
                aces--;
            }

            return total;
        }

        //Hämta värdet av det sista kortet
        public int GetLastCardDamageValue(List<int> cards)
        {
            if (cards.Count == 0)
                throw new InvalidOperationException("Hand has no cards");

            int lastCard = cards.Last();
            int lastValue = GetCardValue(lastCard);

            // Om sista kortet inte är ett ess, är det enkelt
            if (lastValue != 11)
                return lastValue;

            // Sista kortet är ett ess.
            // Räkna handen utan sista kortet.
            var withoutLast = cards.Take(cards.Count - 1).ToList();
            int totalWithoutLast = CalculateHandValue(withoutLast);

            // Om esset kan vara 11 utan att busta, då är det 11 i skada.
            if (totalWithoutLast + 11 <= 21)
                return 11;

            // Annars räknas det som 1
            return 1;
        }

        //Blackjack?
        public bool IsBlackjack(List<int> cards)
        {
            return cards.Count == 2 && CalculateHandValue(cards) == 21;
        }

        //Jämför två händer och returnera vinnande handen
        public List<int> GetWinningHand(List<int> hand1, List<int> hand2)
        {
            int value1 = CalculateHandValue(hand1);
            int value2 = CalculateHandValue(hand2);

            bool bust1 = value1 > 21;
            bool bust2 = value2 > 21;

            if (bust1 && bust2)
                return value1 < value2 ? hand1 : hand2;   // minst bust vinner

            if (bust1) return hand2;
            if (bust2) return hand1;

            return value1 >= value2 ? hand1 : hand2;     // högst vinner
        }
    }
}
