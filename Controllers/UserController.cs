using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Tazuki.Models;

namespace Tazuki.Controllers
{
    public class UserController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult MisPedidos()
        {
            if (!Cookies())
                return RedirectToAction("InicioSesion", "Home");

            DataTable dt = Home_SQL.Mostrar_Pedido(Sesion.Id);
            ViewBag.Orden = dt;
            dt = Home_SQL.Mostrar_Tazas();
            ViewBag.Tazas = dt;
            dt = Admin_SQL.Mostrar_Tamanos_Tazas();
            ViewBag.TamanosTaza = dt;
            DataTable dt_items = Home_SQL.Mostrar_Pedido_Items();
            ViewBag.Items = dt_items;
            ViewBag.IdUser = Sesion.Id;

            return View();
        }

        public bool Cookies()
        {
            var miCookie = HttpContext.Request.Cookies["Tazuky2"];
            if (Home_SQL.ComprobarCookie(miCookie))
            {
                ViewBag.Activo = "1";
                ViewBag.Nombre = Sesion.Nombre;
                ViewBag.rol = Sesion.rol;
                return true;
            }
            return false;
        }
    }
}