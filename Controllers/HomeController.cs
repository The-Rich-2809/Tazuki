using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
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
            DataTable dt = Home_SQL.Mostrar_Tazas();
            ViewBag.Videos = dt;
            return View();
        }
        public IActionResult Catalogo()
        {
            DataTable dt = Home_SQL.Mostrar_Tazas();
            ViewBag.Videos = dt;
            return View();
        }
        public IActionResult Contacto()
        {
            return View();
        }
        public IActionResult Biografia()
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
