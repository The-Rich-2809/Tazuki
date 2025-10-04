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
            dt = Admin_SQL.Mostrar_Tags();
            ViewBag.Tags = dt;
            DataTable TT = Admin_SQL.Mostrar_Tazas_Tags();
            ViewBag.Taza_Tags = TT;
            return View();
        }

        [HttpGet]
        public IActionResult AgregarDiseno()
        {
            ViewBag.ErrorMessage = Datos.Mensaje;
            DataTable dt = Admin_SQL.Mostrar_Tags();
            ViewBag.Tags = dt;
            dt = Admin_SQL.Mostrar_Tamanos_Tazas();
            ViewBag.Tamano = dt;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarDiseno(
     string nombre, string descripcion, string tipoTaza, string[] tags, IFormFile archivo)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "MP4");

            if (archivo == null || archivo.Length == 0)
            {
                TempData["Message"] = "Error: No se ha seleccionado ningún archivo.";
                return RedirectToAction("Index", "Admin");
            }

            var extension = Path.GetExtension(archivo.FileName);
            var allowedExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    { ".mp4", ".mov", ".m4v", ".webm" };

            if (!allowedExt.Contains(extension))
            {
                TempData["Message"] = $"Error: Extensión no permitida ({extension}).";
                return RedirectToAction("Index", "Admin");
            }

            if (archivo.ContentType?.StartsWith("video/", StringComparison.OrdinalIgnoreCase) != true)
            {
                TempData["Message"] =
                    $"Error: El archivo no parece ser de video (MIME: {archivo.ContentType}).";
                return RedirectToAction("Index", "Admin");
            }

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var baseName = string.IsNullOrWhiteSpace(nombre) ? "diseno" : nombre;

            string Slugify(string s)
            {
                var sinExt = Path.GetFileNameWithoutExtension(s);
                var normalized = sinExt.Normalize(NormalizationForm.FormD);
                var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
                var clean = new string(chars.ToArray());
                clean = Regex.Replace(clean, @"[^a-zA-Z0-9\-_]+", "-");
                clean = Regex.Replace(clean, @"-+", "-").Trim('-');
                if (string.IsNullOrEmpty(clean)) clean = "diseno";
                return clean.Length > 50 ? clean[..50] : clean;
            }

            var safeName = Slugify(baseName).ToLowerInvariant();
            var uniqueFileName = $"{safeName}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 1) Rechazar si ya existe
            if (System.IO.File.Exists(filePath))
            {
                TempData["Message"] = $"Error: Ya existe un archivo llamado {uniqueFileName}.";
                Datos.Mensaje = "El diseño ya existe "+ uniqueFileName;
                return RedirectToAction("AgregarDiseno", "Admin");
            }

            // 2) Guardar primero con CreateNew (no sobrescribe jamás)
            try
            {
                await using (var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    await archivo.CopyToAsync(stream);
                }
            }
            catch (IOException)
            {
                // Alguien creó el archivo entre el Exists y el CreateNew
                TempData["Message"] = $"Error: Ya existe un archivo llamado {uniqueFileName}.";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error al guardar el archivo: {ex.Message}";
                return RedirectToAction("Index", "Admin");
            }

            // 3) Insertar en BD; si falla, revertir el archivo guardado
            Datos.Nombre = nombre;
            Datos.descripcion = descripcion;
            Datos.tamanoTaza = tipoTaza;
            Datos.rutaDiseno = $"MP4/{uniqueFileName}";
            Datos.tags = tags;

            if (!Admin_SQL.Agregar_Diseno())
            {
                try { System.IO.File.Delete(filePath); } catch { /* best-effort */ }
                TempData["Message"] = "Error al registrar el diseño en la BD. No se han guardado cambios.";
                return RedirectToAction("AgregarDiseno", "Admin");
            }
            else
            {
                for(int i = 0; i < tags.Length; i++)
                {
                    Admin_SQL.Agregar_Diseno_Tags(tags[i]);
                }
            }

            TempData["Message"] = $"Archivo guardado y diseño registrado: {uniqueFileName}";
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
        [HttpPost]
        public IActionResult ModEtiquetas([FromBody] EtiquetaParaRecibir data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Nombre))
            {
                return BadRequest("El nombre no puede estar vacío.");
            }

            Datos.Id = data.Id;
            Datos.Nombre = data.Nombre;
            if (Admin_SQL.Mod_Tags() == true)
                return Ok(new { message = "Etiqueta actualizada correctamente." });
            else
                return StatusCode(500, "Ocurrió un error en el servidor al intentar actualizar.");
        }
        [HttpGet]
        public IActionResult EliminarEtiquetas(int Id)
        {
            Datos.Id = Id;
            Admin_SQL.Eliminar_Tags();
            return RedirectToAction("Etiquetas", "Admin");
        }

        public IActionResult TamanosTaza()
        {
            ViewBag.ErrorMessage = Datos.Mensaje;
            DataTable dt = Admin_SQL.Mostrar_Tamanos_Tazas();
            ViewBag.Tags = dt;
            return View();
        }
        [HttpPost]
        public IActionResult AgregarTamano(string nombre)
        {
            Datos.Nombre = nombre;
            Admin_SQL.Agregar_Tamano();
            return RedirectToAction("TamanosTaza", "Admin");
        }
        [HttpPost]
        public IActionResult ModTamano([FromBody] EtiquetaParaRecibir data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Nombre))
            {
                return BadRequest("El nombre no puede estar vacío.");
            }

            Datos.Id = data.Id;
            Datos.Nombre = data.Nombre;
            if (Admin_SQL.Mod_Tamano() == true)
                return Ok(new { message = "El tamaño fue actualizado correctamente." });
            else
                return StatusCode(500, "Ocurrió un error en el servidor al intentar actualizar.");
        }
        [HttpGet]
        public IActionResult EliminarTamano(int Id)
        {
            Datos.Id = Id;
            Admin_SQL.Eliminar_Tamano();
            return RedirectToAction("TamanosTaza", "Admin");
        }
    }
}