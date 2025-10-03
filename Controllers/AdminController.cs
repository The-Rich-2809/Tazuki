using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Tazuki.Models;

namespace Tazuki.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            DataTable dt = Admin_SQL.Mostrar_Tazas();
            ViewBag.Videos = dt;
            return View();
        }

        [HttpGet]
        public IActionResult AgregarDiseno()
        {
            DataTable dt = Admin_SQL.Mostrar_Tags();
            ViewBag.Tags = dt;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarDiseno(string nombre, string descripcion, string tipoTaza, string[] tags, IFormFile archivo)
        {
            Datos.rutaDiseno = @"mp4/" + archivo.FileName;

            //var uploadsFolder = @"/home/rich/compartido/Tazuki/wwwroot/mp4";
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "MP4");

            Datos.Nombre = nombre;
            Datos.descripcion = descripcion;
            Datos.tamanoTaza = tipoTaza;

            Admin_SQL.Agregar_Diseno();

            // 1. Validar que se ha enviado un archivo
            if (archivo == null || archivo.Length == 0)
            {
                ViewBag.Message = "Error: No se ha seleccionado ningún archivo.";
                return View();
            }

            // 2. Crear una ruta segura para guardar el archivo
            // Es una buena práctica crear una carpeta "uploads" dentro de wwwroot

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 3. Generar un nombre de archivo único para evitar sobreescrituras y problemas de seguridad
            var uniqueFileName = Path.GetFileName(archivo.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4. Guardar el archivo en el sistema de ficheros
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                // 5. Devolver un mensaje de éxito al usuario
                ViewBag.Message = $"Archivo guardado exitosamente en: {filePath}";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al guardar el archivo: {ex.Message}";
            }

            return RedirectToAction("Index", "Admin");
        }

        public IActionResult Etiquetas()
        {
            ViewBag.ErrorMessage = Datos.Mensaje;
            DataTable dt = Admin_SQL.Mostrar_Tags();
            ViewBag.Tags = dt;
            return View();
        }
        
        [HttpPost]
        public IActionResult AgregarEtiquetas(string nombre)
        {
            Datos.Nombre = nombre;
            Admin_SQL.Agregar_Tags();
            return RedirectToAction("Etiquetas", "Admin");
        }
        [HttpGet]
        public IActionResult EliminarEtiquetas(int Id)
        {
            Datos.Id = Id;
            Admin_SQL.Eliminar_Tags();
            return RedirectToAction("Etiquetas", "Admin");
        }
    }
}
