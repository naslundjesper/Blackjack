using Microsoft.AspNetCore.Mvc;
using Blackjack.Data;
using Blackjack.Models;
using Microsoft.EntityFrameworkCore;

namespace Blackjack.Controllers
{
    public class AccountController : Controller
    {
        private readonly BlackjackDbContext _context;
        public AccountController(BlackjackDbContext context) { _context = context; }

        [HttpGet] public IActionResult Login() => View();
        [HttpGet] public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            if (await _context.Players.AnyAsync(p => p.Username == username))
            {
                ViewBag.Error = "Användarnamnet är upptaget.";
                return View();
            }

            var player = new Player
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password) // Kryptering
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Username == username);

            if (player != null && BCrypt.Net.BCrypt.Verify(password, player.PasswordHash))
            {
                // Spara inloggning i cookies
                Response.Cookies.Append("PlayerID", player.PlayerID.ToString());
                Response.Cookies.Append("PlayerName", player.Username);
                return RedirectToAction("Lobby", "Home");
            }

            ViewBag.Error = "Fel användarnamn eller lösenord.";
            return View();
        }
    }
}