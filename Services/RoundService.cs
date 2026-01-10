using Blackjack.Models;
using System;
using System.Linq;

namespace Blackjack.Services
{
    public class RoundService
    {
        private readonly CardService _cards = new CardService();
        private readonly BlackjackRuleService _rules = new BlackjackRuleService();

        //Starta ny runda
        public Round StartRound(Game game)
        {
            var seed = Random.Shared.Next();

            var round = new Round
            {
                GameID = game.GameID,
                Seed = seed,
                Deck = new byte[7],
                DamageDealt = 0
            };

            round.PlayerHands.Add(new PlayerHand
            {
                PlayerID = game.Player1ID,
                DrawnCards = new(),
                HandValue = 0,
                IsCompleted = false
            });

            round.PlayerHands.Add(new PlayerHand
            {
                PlayerID = game.Player2ID,
                DrawnCards = new(),
                HandValue = 0,
                IsCompleted = false
            });

            return round;
        }

        //Spelare drar kort
        public void PlayerHit(Round round, int playerId)
        {
            var hand = round.PlayerHands.First(h => h.PlayerID == playerId);

            int card = DrawCard(round);
            hand.DrawnCards.Add(card);

            hand.HandValue = _rules.CalculateHandValue(hand.DrawnCards);

            if (_rules.IsBust(hand.DrawnCards))
                hand.IsCompleted = true;
        }

        //Spelare stannar
        public void PlayerStand(Round round, int playerId)
        {
            var hand = round.PlayerHands.First(h => h.PlayerID == playerId);
            hand.IsCompleted = true;
        }

        //Är rundan klar?
        public bool IsRoundFinished(Round round)
        {
            return round.PlayerHands.All(h => h.IsCompleted);
        }

        //Summera rundan
        public void ResolveRound(Game game, Round round)
        {
            var p1 = round.PlayerHands.First(h => h.PlayerID == game.Player1ID);
            var p2 = round.PlayerHands.First(h => h.PlayerID == game.Player2ID);

            var winnerHand = _rules.GetWinningHand(p1.DrawnCards, p2.DrawnCards);
            int loserIndex = _rules.GetLoserIndex(p1.DrawnCards, p2.DrawnCards);

            var loser = loserIndex == 0 ? p1 : p2;
            var winner = loser == p1 ? p2 : p1;

            round.LoserPlayerID = loser.PlayerID;
            round.DamageDealt = _rules.CalculateDamage(winner.DrawnCards);

            // Applicera skada
            if (loser.PlayerID == game.Player1ID)
                game.Player1HP -= round.DamageDealt;
            else
                game.Player2HP -= round.DamageDealt;

            if (game.Player1HP <= 0 || game.Player2HP <= 0)
                game.Status = "Finished";
        }

        //Dra nästa kort
        private int DrawCard(Round round)
        {
            var deck = _cards.CreateDeck();
            _cards.Shuffle(deck, round.Seed);

            for (int i = 0; i < 52; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = i % 8;

                bool drawn = (round.Deck[byteIndex] & (1 << bitIndex)) != 0;

                if (!drawn)
                {
                    round.Deck[byteIndex] |= (byte)(1 << bitIndex);
                    return deck[i];
                }
            }

            throw new InvalidOperationException("Deck is empty");
        }
    }
}
