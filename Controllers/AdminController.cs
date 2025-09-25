using Microsoft.AspNetCore.Mvc;

namespace Tazuki.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AgregarDiseno()
        {
            return View();
        }
    }
}
