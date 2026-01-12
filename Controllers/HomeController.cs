using Microsoft.AspNetCore.Mvc;
using Blackjack.Data;

namespace Blackjack.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlackjackDbContext _context;
        public HomeController(BlackjackDbContext context) { _context = context; }

        public IActionResult Lobby()
        {
            var id = Request.Cookies["PlayerID"];
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Login", "Account");

            ViewBag.PlayerID = id;
            ViewBag.PlayerName = Request.Cookies["PlayerName"];
            return View();
        }

        public IActionResult Game(int gameId)
        {
            ViewBag.PlayerID = Request.Cookies["PlayerID"];
            ViewBag.PlayerName = Request.Cookies["PlayerName"];
            ViewBag.GameID = gameId;
            return View();
        }
    }
}