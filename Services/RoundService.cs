using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Blackjack.Data;
using Blackjack.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack.Services
{
    public class RoundService
    {
        private readonly BlackjackDbContext _context;

        public RoundService(BlackjackDbContext context) => _context = context;

   
        public Card DrawCard(ref long deckVector)
        {
            var random = new Random();
            while (true)
            {
                int index = random.Next(0, 52);
                // Kontrollera om biten på position 'index' är 0
                if ((deckVector & (1L << index)) == 0)
                {
                    // Markera biten som 1 (kortet är draget)
                    deckVector |= (1L << index);
                    return new Card { CardIndex = index };
                }
            }
        }

       
        public void ResolveRound(int gameId)
        {
            // 1. Hämta spelet och senaste rundan
            var game = _context.Games
                .Include(g => g.Rounds)
                .FirstOrDefault(g => g.GameID == gameId);

            if (game == null || !game.Rounds.Any()) return;

            var currentRound = game.Rounds.OrderByDescending(r => r.RoundID).First();

            // 2. Hämta spelarnas händer
            var hands = _context.PlayerHands
                .Where(h => h.RoundID == currentRound.RoundID)
                .ToList();

            var h1 = hands.FirstOrDefault(h => h.PlayerID == game.Player1ID);
            var h2 = hands.FirstOrDefault(h => h.PlayerID == (game.Player2ID ?? -1));

            if (h1 == null || h2 == null) return;

            // 3. Logik för att utse vinnare (närmast 21 utan att gå över)
            int winnerId = 0;
            PlayerHand? winningHand = null;

            if (h1.HandValue > 21 && h2.HandValue > 21)
            {
                // Båda bust - ingen vinner denna runda
                return;
            }
            else if (h1.HandValue > 21)
            {
                winnerId = (game.Player2ID ?? -1);
                winningHand = h2;
            }
            else if (h2.HandValue > 21)
            {
                winnerId = game.Player1ID;
                winningHand = h1;
            }
            else if (h1.HandValue > h2.HandValue)
            {
                winnerId = game.Player1ID;
                winningHand = h1;
            }
            else if (h2.HandValue > h1.HandValue)
            {
                winnerId = (game.Player2ID ?? -1);
                winningHand = h2;
            }
            else
            {
                // Oavgjort (Push)
                return;
            }

            // 4. Beräkna skada baserat på sista kortet i vinnarens hand
            if (winningHand != null && winnerId != -1)
            {
                // Fixar null-varning vid deserialisering
                var cards = JsonConvert.DeserializeObject<List<Card>>(winningHand.DrawnCardsJson ?? "[]") ?? new List<Card>();

                if (cards.Any())
                {
                    // Hämta värdet från vinnarens SISTA dragna kort
                    var lastCard = cards.Last();
                    int damage = lastCard.Value;

                    // Om vinnaren har Blackjack (21), dela ut dubbel skada
                    if (winningHand.HandValue == 21)
                    {
                        damage *= 2;
                    }

                    // 5. Dra av HP från förloraren
                    if (winnerId == game.Player1ID)
                    {
                        game.Player2HP -= damage;
                    }
                    else
                    {
                        game.Player1HP -= damage;
                    }

                    // Logga resultatet på rundan
                    currentRound.DamageDealt = damage;
                    currentRound.LoserPlayerID = (winnerId == game.Player1ID) ? (game.Player2ID ?? -1) : game.Player1ID;
                }
            }

            // 6. Kontrollera om spelet är slut
            if (game.Player1HP <= 0 || game.Player2HP <= 0)
            {
                game.Status = "Finished";
            }

            _context.SaveChanges();
        }
    }
}