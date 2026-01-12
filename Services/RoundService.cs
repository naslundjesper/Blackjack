using Microsoft.EntityFrameworkCore;
using Blackjack.Data;
using Blackjack.Models;

namespace Blackjack.Services
{
    public class RoundService
    {
        private readonly BlackjackDbContext _context;

        public RoundService(BlackjackDbContext context) => _context = context;

        public void ResolveRound(int gameId)
        {
            var game = _context.Games
                .Include(g => g.Rounds)
                .ThenInclude(r => r.PlayerHands)
                .FirstOrDefault(g => g.GameID == gameId);

            if (game == null || !game.Rounds.Any()) return;

            var currentRound = game.Rounds.OrderByDescending(r => r.RoundID).First();
            var h1 = currentRound.PlayerHands.FirstOrDefault(h => h.PlayerID == game.Player1ID);
            var h2 = currentRound.PlayerHands.FirstOrDefault(h => h.PlayerID == (game.Player2ID ?? -1));

            if (h1 == null || h2 == null) return;

            int winnerId = -1;
            PlayerHand? winningHand = null;

            // 1. BESTÄM VINNARE (Endast om man är under eller lika med 21)
            bool p1Bust = h1.HandValue > 21;
            bool p2Bust = h2.HandValue > 21;

            if (p1Bust && p2Bust)
            {
                // Båda bustade - Ingen vinner, ingen tar skada
                return;
            }
            else if (p1Bust)
            {
                // P1 bustade, P2 vinner (eftersom P2 inte bustade)
                winnerId = game.Player2ID ?? -1;
                winningHand = h2;
            }
            else if (p2Bust)
            {
                // P2 bustade, P1 vinner
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
                winnerId = game.Player2ID ?? -1;
                winningHand = h2;
            }
            else
            {
                // Oavgjort (Push) - Ingen skada
                return;
            }

            // 2. APPLICERA SKADA (Körs endast om vi har en vinnare som INTE bustat)
            if (winningHand != null && winnerId != -1)
            {
                var cards = winningHand.DrawnCards;

                if (cards.Any())
                {
                    var lastCard = new Card { CardIndex = cards.Last() };
                    int damage = lastCard.Value;

                    // Dubbel skada vid exakt 21
                    if (winningHand.HandValue == 21) damage *= 2;

                    if (winnerId == game.Player1ID)
                    {
                        game.Player2HP -= damage;
                    }
                    else
                    {
                        game.Player1HP -= damage;
                    }

                    currentRound.DamageDealt = damage;
                    currentRound.LoserPlayerID = (winnerId == game.Player1ID) ? (game.Player2ID ?? -1) : game.Player1ID;
                }
            }

            // 3. KOLLA OM MATCHEN ÄR SLUT
            if (game.Player1HP <= 0 || game.Player2HP <= 0)
            {
                game.Status = "Finished";
            }

            _context.SaveChanges();
        }
    }
}