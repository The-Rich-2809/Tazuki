﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
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

        [HttpGet] //Mostrar diseños 
        public IActionResult Index()
        {
            DataTable dt = Admin_SQL.Mostrar_Tazas();
            ViewBag.Videos = dt;
            dt = Admin_SQL.Mostrar_Tags();
            ViewBag.Tags = dt;
            DataTable TT = Admin_SQL.Mostrar_Tazas_Tags();
            ViewBag.Taza_Tags = TT;
            dt = Admin_SQL.Mostrar_Tamanos_Tazas();
            ViewBag.Tamano = dt;
            return View();
        }

        [HttpPost] // Funcion para actualizar el si esta actvo
        public IActionResult ActDesDiseno([FromBody] EtiquetaParaRecibir model)
        {
            Datos.Id = model.Id;
            int Activo = 0;
            if (model.Activo == true)
                Activo = 1;

            Admin_SQL.Mod_ActDesDiseno(Activo);
            return Ok();
        }


        [HttpGet] //Agregar diseños
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
        public async Task<IActionResult> AgregarDiseno(string nombre, string descripcion, string tipoTaza, double precio, string[] tags, IFormFile archivo)
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
                Datos.Mensaje = "El diseño ya existe " + uniqueFileName;
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
            Datos.precio = precio;
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
                for (int i = 0; i < tags.Length; i++)
                {
                    Admin_SQL.Agregar_Diseno_Tags(tags[i]);
                }
            }

            TempData["Message"] = $"Archivo guardado y diseño registrado: {uniqueFileName}";
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet] //Agregar diseños (Se agregan los datos desde el archivo)
        public IActionResult AgregarDisenoArchivo()
        {
            ViewBag.ErrorMessage = Datos.Mensaje;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarDisenoArchivo(string nombre, IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ViewBag.ErrorMessage = "Debes seleccionar un archivo de video.";
                return PartialView("_AlertaDiseno");
            }

            var extension = Path.GetExtension(archivo.FileName);
            var allowedExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { ".mp4", ".mov", ".m4v", ".webm" };
            if (!allowedExt.Contains(extension))
            {
                ViewBag.ErrorMessage = $"Error: Extensión no permitida ({extension}).";
                return PartialView("_AlertaDiseno");
            }
            if (archivo.ContentType?.StartsWith("video/", StringComparison.OrdinalIgnoreCase) != true)
            {
                ViewBag.ErrorMessage = $"Error: El archivo no parece ser de video (MIME: {archivo.ContentType}).";
                return PartialView("_AlertaDiseno");
            }

            // Guardar en carpeta temporal (una sola vez)
            var tempFolder = Path.Combine(_webHostEnvironment.WebRootPath, "_tmp");
            Directory.CreateDirectory(tempFolder);

            var uploadToken = Guid.NewGuid().ToString("N");
            var tempFileName = uploadToken + extension;
            var tempPath = Path.Combine(tempFolder, tempFileName);

            await using (var fs = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await archivo.CopyToAsync(fs);
            }

            // --- Tu lógica de frases / tags ---
            string[] frases = archivo.FileName.Split(new[] { " - " }, StringSplitOptions.None);
            string[,] frasesBuscadas = new string[frases.Length, 3];
            for (int i = 0; i < frases.Length; i++)
            {
                var texto = frases[i]
                    .Replace(".mp4", "", StringComparison.OrdinalIgnoreCase)
                    .Replace(".mov", "", StringComparison.OrdinalIgnoreCase)
                    .Replace(".m4v", "", StringComparison.OrdinalIgnoreCase)
                    .Replace(".webm", "", StringComparison.OrdinalIgnoreCase);

                frasesBuscadas[i, 0] = texto;

                var dt = Admin_SQL.Buscar_Tags(frases[i]); // evita llamarlo dos veces
                if (dt.Rows.Count > 0)
                {
                    frasesBuscadas[i, 1] = "Encontrada";
                    frasesBuscadas[i, 2] = dt.Rows[0][0].ToString();
                }
                else
                {
                    frasesBuscadas[i, 1] = "No encontrada";
                    frasesBuscadas[i, 2] = "-1";
                }

                if (i == frases.Length - 1)
                {
                    Datos.descripcion = texto;
                    frasesBuscadas[i, 1] = "Descripción";
                }
                    
            }

            // Pasa token/ext (ideal: como hidden inputs; si sigues usando Datos.*, al menos setéalos una vez)
            ViewBag.Nombre = nombre;
            ViewBag.MensajeConfirmacion = $"¿Estás seguro de agregar el diseño: {nombre}?";
            ViewBag.Tags = frasesBuscadas;
            ViewBag.UploadToken = uploadToken;
            ViewBag.UploadExt = extension;

            // Si vas a usar Datos.* entre peticiones, setéalos aquí (ojo: no es multiusuario-safe)
            Datos.UploadExt = extension;
            Datos.UploadToken = uploadToken;
            Datos.tamanoTaza = nombre;
            Datos.Etiquetas = frasesBuscadas;
            Datos.Nombre = nombre; // <-- para que no sea null al Slugify del siguiente paso

            return PartialView("_AlertaDiseno");
        }

        [HttpPost] //Agregar diseños (Se agregan los datos desde el archivo)
        public async Task<IActionResult> GuardarDisenoConfirmado()
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "MP4");
            var tempFolder = Path.Combine(_webHostEnvironment.WebRootPath, "_tmp");
            Directory.CreateDirectory(uploadsFolder);

            if (string.IsNullOrWhiteSpace(Datos.UploadToken) || string.IsNullOrWhiteSpace(Datos.UploadExt))
            {
                TempData["Message"] = "Error: Archivo temporal no encontrado o expirado.";
                return RedirectToAction("Index", "Admin");
            }

            var tempPath = Path.Combine(tempFolder, Datos.UploadToken + Datos.UploadExt);
            if (!System.IO.File.Exists(tempPath))
            {
                TempData["Message"] = "Error: El archivo temporal ya no existe.";
                return RedirectToAction("Index", "Admin");
            }

            // Construye el nombre final como ya lo hacías (Slugify, etc.)
            var baseName = string.IsNullOrWhiteSpace(Datos.Nombre) ? "diseno" : Datos.Nombre;
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
            var uniqueFileName = $"{safeName}{Datos.UploadExt}";
            var finalPath = Path.Combine(uploadsFolder, uniqueFileName);

            if (System.IO.File.Exists(finalPath))
            {
                TempData["Message"] = $"Error: Ya existe un archivo llamado {uniqueFileName}.";
                try { System.IO.File.Delete(tempPath); } catch { }
                return RedirectToAction("AgregarDiseno", "Admin");
            }

            // Mover de temporal a definitivo (atómico en la misma unidad)
            System.IO.File.Move(tempPath, finalPath);

            // ---- Inserción en BD (igual que antes)
            Datos.rutaDiseno = $"MP4/{uniqueFileName}";
            Datos.precio = 50.00; // Precio fijo por ahora
            Datos.tamanoTaza = "1"; // Tamaño fijo por ahora

            // Asegúrate de tener seteados Datos.Nombre / Datos.precio / Datos.tamanoTaza según tu UI
            if (!Admin_SQL.Agregar_Diseno())
            {
                try { System.IO.File.Delete(finalPath); } catch { }
                TempData["Message"] = "Error al registrar el diseño en la BD. No se han guardado cambios.";
                return RedirectToAction("AgregarDiseno", "Admin");
            }

            for (int i = 0; i < Datos.Etiquetas.GetLength(0); i++)
            {
                if (Datos.Etiquetas[i, 1] == "No encontrada")
                {
                    Datos.Nombre = Datos.Etiquetas[i, 0]; // nombre del tag
                    int nuevoId = Admin_SQL.Agregar_Tags(); // <-- que te devuelva el ID
                    Datos.Etiquetas[i, 2] = nuevoId.ToString();
                }
            }
            for (int i = 0; i < Datos.Etiquetas.GetLength(0); i++)
            {
                if (Datos.Etiquetas[i, 1] != "Descripción")
                {
                    Admin_SQL.Agregar_Diseno_Tags(Datos.Etiquetas[i, 2]);
                }
            }
            
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public IActionResult ModDiseno(string Id)
        {
            ViewBag.ErrorMessage = Datos.Mensaje;

            DataTable dt_Tazas = Admin_SQL.Mostrar_Tazas(); //Mostrar diseños de tazas
            DataTable dt_Tags = Admin_SQL.Mostrar_Tags(); //Mostrar los tags (etiquetas)
            ViewBag.Tags = dt_Tags;
            DataTable dt_Tamanos = Admin_SQL.Mostrar_Tamanos_Tazas(); //Mostrar los tamaños de tazas
            ViewBag.Tamano = dt_Tamanos;

            DataTable TT = Admin_SQL.Mostrar_Tazas_Tags_Id(Id); //Mostrar Tabla intermedia Tags y Diseños
            ViewBag.TT = TT;

            foreach (DataRow row_taza in dt_Tazas.Rows) //Permite buscar los datos del diseño
            {
                if (row_taza[0].ToString() == Id)
                {
                    ViewBag.Taza = row_taza;
                    break;
                }
            }

            return View();
        }

        [HttpPost]
        public IActionResult ModDiseno(int Id)
        {
            return RedirectToAction("Index", "Admin");
        }
        [HttpGet]
        public IActionResult EliminarDiseno(string Id)
        {
            ViewBag.ErrorMessage = Datos.Mensaje;

            DataTable dt_Tazas = Admin_SQL.Mostrar_Tazas();
            DataTable dt_Tags = Admin_SQL.Mostrar_Tags();
            DataTable TT = Admin_SQL.Mostrar_Tazas_Tags_Id(Id);
            string[] Tags = new string[TT.Rows.Count];
            ViewBag.Count_Tags = TT.Rows.Count;
            DataTable dt_Tamanos = Admin_SQL.Mostrar_Tamanos_Tazas();

            foreach (DataRow row_taza in dt_Tazas.Rows)
            {
                if (row_taza[0].ToString() == Id)
                {
                    ViewBag.Taza = row_taza;
                    Datos.rutaDiseno = row_taza[5].ToString().Remove(0, 4);
                    break;
                }
            }

            int i = 0;
            foreach (DataRow row_TT in TT.Rows)
            {
                foreach (DataRow row_tag in dt_Tags.Rows)
                {
                    if (row_tag[0].ToString() == row_TT[1].ToString())
                    {
                        Tags[i] = row_tag[1].ToString();
                        i++;
                    }
                }
            }


            ViewBag.Tags_Taza = Tags;

            return View();
        }
        [HttpPost]
        public IActionResult EliminarDiseno(int Id)
        {
            Datos.Id = Id;
            Admin_SQL.Eliminar_Diseno();
            Admin_SQL.Eliminar_Diseno_Tags();

            try
            {

                // --- 3. Construir la ruta completa y segura ---
                // Combina la carpeta de uploads (ej. wwwroot/imagenes/uploads) con el nombre del archivo.
                var carpetaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "mp4");
                var rutaCompleta = Path.Combine(carpetaUploads, Datos.rutaDiseno);

                // --- 4. Verificar si el archivo existe antes de borrar ---
                if (System.IO.File.Exists(rutaCompleta))
                {
                    // --- 5. Eliminar el archivo ---
                    System.IO.File.Delete(rutaCompleta);
                    TempData["Exito"] = $"El archivo '{Datos.rutaDiseno}' fue eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] = $"El archivo '{Datos.rutaDiseno}' no existe.";
                }
            }
            catch (Exception ex)
            {
                // Es una buena práctica registrar el error (ex.Message)
                TempData["Error"] = "Ocurrió un error inesperado al eliminar el archivo.";
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