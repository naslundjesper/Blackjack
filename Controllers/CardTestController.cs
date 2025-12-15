using Blackjack.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blackjack.Controllers
{
    [ApiController]
    [Route("api/test/cards")]
    public class CardTestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Test()
        {
            var cardService = new CardService();

            var deck = cardService.CreateDeck();
            cardService.Shuffle(deck, seed: 123);

            int card1 = cardService.DrawCard(deck);
            int card2 = cardService.DrawCard(deck);

            return Ok(new
            {
                FirstCard = card1,
                SecondCard = card2,
                RemainingCards = deck.Count
            });
        }
    }
}
