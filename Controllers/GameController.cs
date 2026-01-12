using Blackjack.Data;
using Blackjack.Models;
using Blackjack.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blackjack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly BlackjackDbContext _context;
        private readonly BlackjackRuleService _ruleService;
        private readonly GameService _gameService;
        private readonly CardService _cardService;
        private readonly RoundService _roundService;

        public GameController(
            BlackjackDbContext context,
            BlackjackRuleService ruleService,
            GameService gameService,
            CardService cardService,
            RoundService roundService)
        {
            _context = context;
            _ruleService = ruleService;
            _gameService = gameService;
            _cardService = cardService;
            _roundService = roundService;
        }

        [HttpPost("setup")]
        public async Task<IActionResult> SetupGame([FromQuery] string p1Name, [FromQuery] string lobbyCode)
        {
            var existingGame = await _context.Games
                .FirstOrDefaultAsync(g => g.LobbyCode == lobbyCode.ToUpper() && g.Status == "Waiting");

            if (existingGame != null) return BadRequest("Lobbyn finns redan.");

            var p1 = await _context.Players.FirstOrDefaultAsync(p => p.Username == p1Name);
            if (p1 == null) return NotFound("Spelare ej hittad.");

            // Använder GameService för att skapa spelobjektet
            var game = _gameService.CreateNewGame(p1.PlayerID, lobbyCode);

            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return Ok(new { game.GameID, game.LobbyCode });
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinGame([FromQuery] string code, [FromQuery] int playerId)
        {
            var game = await _context.Games
                .FirstOrDefaultAsync(g => g.LobbyCode == code.ToUpper() && g.Status == "Waiting");

            if (game == null) return NotFound("Hittade ingen lobby med den koden.");

            game.Player2ID = playerId;
            game.Status = "InProgress";

            // Viktigt: Sätt även TurnOrder här om du vill att P1 ska börja
            game.TurnOrder = game.Player1ID;

            await _context.SaveChangesAsync();

            var newRound = new Round { GameID = game.GameID, Deck = new byte[7] };
            _context.Rounds.Add(newRound);
            await _context.SaveChangesAsync();

            var hand1 = new PlayerHand { RoundID = newRound.RoundID, PlayerID = game.Player1ID };
            // ÄNDRING: Använd playerId direkt här för att undvika 500-fel
            var hand2 = new PlayerHand { RoundID = newRound.RoundID, PlayerID = playerId };

            _context.PlayerHands.AddRange(hand1, hand2);
            await _context.SaveChangesAsync();

            return Ok(new { gameID = game.GameID });
        }

        [HttpGet("{id}/state")]
        public async Task<IActionResult> GetState(int id)
        {
            var game = await _context.Games
                .Include(g => g.Rounds).ThenInclude(r => r.PlayerHands)
                .FirstOrDefaultAsync(g => g.GameID == id);

            if (game == null) return NotFound();

            var currentRound = game.Rounds.OrderByDescending(r => r.RoundID).FirstOrDefault();
            var p1Hand = currentRound?.PlayerHands.FirstOrDefault(h => h.PlayerID == game.Player1ID);
            var p2Hand = currentRound?.PlayerHands.FirstOrDefault(h => h.PlayerID == (game.Player2ID ?? 0));

            return Ok(new
            {
                status = game.Status,
                player1HP = game.Player1HP,
                player2HP = game.Player2HP,
                p1Sum = p1Hand?.HandValue ?? 0,
                p2Sum = p2Hand?.HandValue ?? 0,
                p1Cards = p1Hand?.DrawnCards ?? new List<int>(),
                p2Cards = p2Hand?.DrawnCards ?? new List<int>(),
                turnOrder = game.TurnOrder,
                player1ID = game.Player1ID,
                player2ID = game.Player2ID
            });
        }

        [HttpPost("{gameId}/hit/{playerId}")]
        public async Task<IActionResult> Hit(int gameId, int playerId)
        {
            var game = await GetFullGame(gameId);
            if (game == null || game.TurnOrder != playerId || game.Status == "Finished") return BadRequest();

            var currentRound = game.Rounds.OrderByDescending(r => r.RoundID).First();
            var hand = currentRound.PlayerHands.First(h => h.PlayerID == playerId);
            var opponentHand = currentRound.PlayerHands.First(h => h.PlayerID != playerId);

            // 1. Dra kort via CardService
            int cardIndex = _cardService.DrawCardFromDeck(currentRound.Deck);

            var cards = hand.DrawnCards;
            cards.Add(cardIndex);
            hand.DrawnCards = cards;

            // 2. Beräkna värde via RuleService
            hand.HandValue = _ruleService.CalculateHandValue(cards);

            if (hand.HandValue > 21) hand.IsCompleted = true;

            // 3. Turskifte direkt 
            if (!opponentHand.IsCompleted)
            {
                game.TurnOrder = opponentHand.PlayerID;
            }

            await CheckAndResolve(game, currentRound);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{gameId}/stand/{playerId}")]
        public async Task<IActionResult> Stand(int gameId, int playerId)
        {
            var game = await GetFullGame(gameId);
            if (game == null || game.TurnOrder != playerId) return BadRequest();

            var currentRound = game.Rounds.OrderByDescending(r => r.RoundID).First();
            var hand = currentRound.PlayerHands.First(h => h.PlayerID == playerId);
            var opponentHand = currentRound.PlayerHands.First(h => h.PlayerID != playerId);

            hand.IsCompleted = true;

            // Om motståndaren kan spela, byt tur. Annars ligger turen kvar tills rundan avgörs.
            if (!opponentHand.IsCompleted)
            {
                game.TurnOrder = opponentHand.PlayerID;
            }

            await CheckAndResolve(game, currentRound);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private async Task CheckAndResolve(Game game, Round round)
        {
            if (round.PlayerHands.All(h => h.IsCompleted))
            {
                _roundService.ResolveRound(game.GameID);

                if (game.Status != "Finished")
                {
                   
                    var nextRound = new Round { GameID = game.GameID, Deck = new byte[7] };
                    _context.Rounds.Add(nextRound);
                    await _context.SaveChangesAsync();

                    _context.PlayerHands.Add(new PlayerHand { RoundID = nextRound.RoundID, PlayerID = game.Player1ID });
                    _context.PlayerHands.Add(new PlayerHand { RoundID = nextRound.RoundID, PlayerID = game.Player2ID!.Value });

                    game.TurnOrder = game.Player1ID;
                }
            }
        }

        private async Task<Game?> GetFullGame(int id)
        {
            return await _context.Games
                .Include(g => g.Rounds).ThenInclude(r => r.PlayerHands)
                .FirstOrDefaultAsync(g => g.GameID == id);
        }
    }
}