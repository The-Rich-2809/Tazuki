using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tazuki.Models;

namespace Tazuki.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string[] tazas = { "mp4/Dragon_Ball.mp4", "mp4/taza1.mp4", "mp4/Amor.mp4", "mp4/taza4.mp4", "mp4/Caballeros.mp4", "mp4/Toluca.mp4" };
            ViewBag.Tazas = tazas;
            return View();
        }
        public IActionResult Catalogo()
        {
            return View();
        }
        public IActionResult Contacto()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
