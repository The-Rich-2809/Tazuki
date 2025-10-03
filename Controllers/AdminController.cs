using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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
        public async Task<IActionResult> AgregarDiseno(
    string nombre,
    string descripcion,
    string tipoTaza,
    string[] tags,
    IFormFile archivo)
        {
            // Carpeta física donde se guardará el archivo
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "MP4");

            // 1) Validaciones básicas
            if (archivo == null || archivo.Length == 0)
            {
                TempData["Message"] = "Error: No se ha seleccionado ningún archivo.";
                return RedirectToAction("Index", "Admin");
            }

            // Validar extensión/MIME (ajusta la lista según tu caso)
            var extension = Path.GetExtension(archivo.FileName);
            var allowedExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    { ".mp4", ".mov", ".m4v", ".webm" };

            if (!allowedExt.Contains(extension))
            {
                TempData["Message"] = $"Error: Extensión no permitida ({extension}).";
                return RedirectToAction("Index", "Admin");
            }

            // (Opcional) validar MIME
            if (archivo.ContentType?.StartsWith("video/", StringComparison.OrdinalIgnoreCase) != true)
            {
                TempData["Message"] = $"Error: El archivo no parece ser de video (MIME: {archivo.ContentType}).";
                return RedirectToAction("Index", "Admin");
            }

            // 2) Asegurar carpeta
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 3) Crear nombre basado en "nombre" (slug) + timestamp + extensión
            //    - Evita caracteres problemáticos
            //    - Evita colisiones
            var baseName = string.IsNullOrWhiteSpace(nombre) ? "diseno" : nombre;

            string Slugify(string s)
            {
                var sinExt = Path.GetFileNameWithoutExtension(s);
                // Reemplaza espacios por guiones, elimina caracteres no válidos
                var normalized = sinExt.Normalize(NormalizationForm.FormD);
                var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
                var clean = new string(chars.ToArray());
                clean = Regex.Replace(clean, @"[^a-zA-Z0-9\-_]+", "-");
                clean = Regex.Replace(clean, @"-+", "-").Trim('-');
                if (string.IsNullOrEmpty(clean)) clean = "diseno";
                // Limitar largo para evitar nombres gigantes
                return clean.Length > 50 ? clean[..50] : clean;
            }

            var safeName = Slugify(baseName).ToLowerInvariant();
            var uniqueFileName = $"{safeName}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4) Guardar físicamente
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await archivo.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error al guardar el archivo: {ex.Message}";
                return RedirectToAction("Index", "Admin");
            }

            // 5) Guardar en BD *después* de conocer el nombre final
            //    (asegúrate de que Admin_SQL.Agregar_Diseno() use estos campos)
            Datos.Nombre = nombre;
            Datos.descripcion = descripcion;
            Datos.tamanoTaza = tipoTaza;
            Datos.rutaDiseno = $"MP4/{uniqueFileName}"; // usa misma mayúscula/minúscula que la carpeta
            Datos.tags = tags;

            try
            {
                Admin_SQL.Agregar_Diseno();
                TempData["Message"] = $"Archivo guardado y diseño registrado: {uniqueFileName}";
            }
            catch (Exception ex)
            {
                // Si falla BD, podrías opcionalmente borrar el archivo para no dejar huérfanos
                try { System.IO.File.Delete(filePath); } catch { /* ignorar */ }
                TempData["Message"] = $"Error al registrar en BD: {ex.Message}";
            }

            // PRG pattern: usa TempData para mostrar el mensaje en Index
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
