using Microsoft.AspNetCore.Mvc;

namespace Tazuki.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Videos()
        {
            return View();
        }
    }
}
