using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Blackjack.Data;
using Blackjack.Models;
using Blackjack.Services;

namespace Blackjack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly BlackjackDbContext _context;
        private readonly RoundService _roundService;

        public GameController(BlackjackDbContext context, RoundService roundService)
        {
            _context = context;
            _roundService = roundService;
        }

        [HttpPost("setup")]
        public async Task<IActionResult> SetupGame(string p1Name, string p2Name)
        {
            // 1. Skapa och spara spelare
            var p1 = new Player { Username = p1Name, PasswordHash = "default" };
            var p2 = new Player { Username = p2Name, PasswordHash = "default" };

            _context.Players.AddRange(p1, p2);
            await _context.SaveChangesAsync();

            // 2. Skapa spelet med ALLA obligatoriska fält
            var game = new Game
            {
                Player1ID = p1.PlayerID,
                Player2ID = p2.PlayerID,
                Player1HP = 100,
                Player2HP = 100,
                StartHP = 100,              
                LobbyCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper(), // Skapar en unik kod
                TurnOrder = 1,              
                Status = "InProgress"
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync(); // Detta var rad 42 som kraschade tidigare

            // 3. Skapa första rundan
            long initialDeck = 0; // Bitvektor för tom lek (eller startvärde)
            var round = new Round
            {
                GameID = game.GameID,
                Deck = BitConverter.GetBytes(initialDeck),
                DamageDealt = 0,
                Seed = new Random().Next(), 
                LoserPlayerID = 0           
            };

            _context.Rounds.Add(round);
            await _context.SaveChangesAsync();

            // 4. Skapa händer för båda spelarna
            var h1 = new PlayerHand
            {
                RoundID = round.RoundID,
                PlayerID = p1.PlayerID,
                DrawnCardsJson = "[]",
                HandValue = 0,
                IsCompleted = false
            };
            var h2 = new PlayerHand
            {
                RoundID = round.RoundID,
                PlayerID = p2.PlayerID,
                DrawnCardsJson = "[]",
                HandValue = 0,
                IsCompleted = false
            };

            _context.PlayerHands.AddRange(h1, h2);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                game.GameID,
                game.LobbyCode,
                Player1ID = p1.PlayerID,
                Player2ID = p2.PlayerID
            });
        }

        [HttpPost("{id}/hit/{playerId}")]
        public async Task<IActionResult> Hit(int id, int playerId)
        {
            var game = await _context.Games.Include(g => g.Rounds).FirstOrDefaultAsync(g => g.GameID == id);
            if (game == null) return NotFound("Spelet hittades inte.");

            var round = game.Rounds.OrderByDescending(r => r.RoundID).FirstOrDefault();
            if (round == null) return BadRequest("Ingen runda hittades.");

            var hand = await _context.PlayerHands.FirstOrDefaultAsync(h => h.RoundID == round.RoundID && h.PlayerID == playerId);
            if (hand == null) return BadRequest("Hand hittades inte.");

            // Bitvektor logik via RoundService
            long deckVector = BitConverter.ToInt64(round.Deck, 0);
            var card = _roundService.DrawCard(ref deckVector);
            round.Deck = BitConverter.GetBytes(deckVector);

            // Deserialisera kortlistan
            var cardsJson = hand.DrawnCardsJson ?? "[]";
            var cards = JsonConvert.DeserializeObject<List<Card>>(cardsJson) ?? new List<Card>();

            cards.Add(card);
            hand.DrawnCardsJson = JsonConvert.SerializeObject(cards);
            hand.HandValue += card.Value;

            await _context.SaveChangesAsync();
            return Ok(new { hand.HandValue, lastCard = $"Kort: {card.RankName} av {card.Suit}" });
        }

        [HttpPost("{id}/stand/{playerId}")]
        public async Task<IActionResult> Stand(int id, int playerId)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameID == id);
            if (game == null) return NotFound();

            if (game.Player2ID.HasValue && playerId == game.Player2ID.Value)
            {
                _roundService.ResolveRound(id);
                return Ok(new { message = "Rundan avslutad och skada beräknad!" });
            }

            return Ok(new { message = "Spelare 1 stannade. Väntar på spelare 2." });
        }

        [HttpGet("{id}/state")]
        public async Task<IActionResult> GetState(int id)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameID == id);
            if (game == null) return NotFound();

            return Ok(new { game.Player1HP, game.Player2HP, game.Status });
        }
    }
}