using System;

namespace Blackjack.Services
{
    public class CardService
    {
        
        public int DrawCardFromDeck(byte[] deck)
        {
            var random = new Random();
            int attempts = 0;
            const int maxCards = 52;

            while (attempts < maxCards)
            {
                // Slumpa ett index mellan 0 och 51
                int cardIndex = random.Next(0, maxCards);

                // Hitta vilken byte (0-6) och vilken bit i den byten (0-7) som motsvarar kortet
                int byteIndex = cardIndex / 8;
                int bitIndex = cardIndex % 8;

                // Kontrollera om biten är 0 (kortet finns kvar i leken)
                // (1 << bitIndex) skapar en mask, t.ex. 00000100 för bit 2.
                if ((deck[byteIndex] & (1 << bitIndex)) == 0)
                {
                    // Markera kortet som draget genom att sätta biten till 1 med OR-operatorn
                    deck[byteIndex] |= (byte)(1 << bitIndex);

                    return cardIndex;
                }

                attempts++;
            }

            throw new InvalidOperationException("Kortleken är tom! Inga fler kort kan dras i denna runda.");
        }

        /// Hjälpmetod för att kontrollera hur många kort som finns kvar i leken.
        public int GetRemainingCardsCount(byte[] deck)
        {
            int count = 0;
            for (int i = 0; i < 52; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = i % 8;
                if ((deck[byteIndex] & (1 << bitIndex)) == 0)
                {
                    count++;
                }
            }
            return count;
        }
    }
}