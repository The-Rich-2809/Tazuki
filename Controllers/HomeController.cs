using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using Tazuki.Models;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Tazuki.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult InicioSesion()
        {
            Cookies();
            return View();
        }
        [HttpPost]
        public IActionResult InicioSesion(User user)
        {
            Cookies();

            if (Home_SQL.Ingresar(user))
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(365);
                options.IsEssential = true;
                options.Path = "/";
                HttpContext.Response.Cookies.Append("Tazuky2", user.email, options);

                return RedirectToAction("Index", "Home");
            }
            else
                ViewBag.ErrorMessage = "Correo electrónico o contraseña incorrectos.";
            return View();
        }
        public IActionResult Registro()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registro(User user)
        {
            if (Home_SQL.Buscar_User(user.email))
            {
                if (user.password == user.password2)
                {
                    if (Home_SQL.Agregar_User(user))
                        return RedirectToAction("InicioSesion", "Home");
                    else
                    {
                        ViewBag.ErrorMessage = "Error al crear el usuario. Inténtalo de nuevo.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Las contraseñas no coinciden.";
                }
            }
            else
                ViewBag.ErrorMessage = "El correo electrónico ya está registrado.";

            return View();
        }

        public IActionResult Index()
        {
            Cookies();
            DataTable dt = Home_SQL.Mostrar_Tazas();
            ViewBag.Videos = dt;
            return View();
        }
        public IActionResult Catalogo()
        {
            Cookies();
            DataTable dt = Home_SQL.Mostrar_Tazas();
            ViewBag.Videos = dt;
            dt = Home_SQL.Mostrar_Tags();
            ViewBag.Tags = dt;
            dt = Home_SQL.Mostrar_Tazas_Tags();
            ViewBag.DisenoTags = dt;
            return View();
        }
        public IActionResult Producto(string id)
        {
            Cookies();
            ViewBag.ErrorMessage = Datos.Mensaje;
            DataTable dt_Tazas = Admin_SQL.Mostrar_Tazas();
            DataTable dt_Tags = Admin_SQL.Mostrar_Tags();
            DataTable TT = Admin_SQL.Mostrar_Tazas_Tags_Id(id);
            string[] Tags = new string[TT.Rows.Count];
            ViewBag.Count_Tags = TT.Rows.Count;
            DataTable dt_Tamanos = Admin_SQL.Mostrar_Tamanos_Tazas();
            ViewBag.Tamanos = dt_Tamanos;

            foreach (DataRow row_taza in dt_Tazas.Rows)
            {
                if (row_taza[0].ToString() == id)
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
        public IActionResult Producto(Carrito carrito)
        {
            if (!Cookies())
                return RedirectToAction("InicioSesion", "Home");

            carrito.Id_User = Sesion.Id;
            if (Home_SQL.Comprobar_Carrito(carrito))
            {
                carrito.Cantidad += Datos.Cantidad;
                Home_SQL.Modificar_Carrito_Cantidad(carrito);
            }

            else
                Home_SQL.Agregar_Carrito(carrito);

            return RedirectToAction("Carrito", "Home");
        }
        // (Asegúrate de tener la clase 'CartItemViewModel' accesible en este archivo)
        // (Y los 'usings' para System.Data, System.Collections.Generic, etc.)

        public IActionResult Carrito()
        {
            if (!Cookies())
                return RedirectToAction("InicioSesion", "Home");
            try
            {
                DataTable dtCarrito = Home_SQL.Mostrar_Carrito(Sesion.Id);
                DataTable dtTamanos = Admin_SQL.Mostrar_Tamanos_Tazas();
                DataTable dtTazas = Home_SQL.Mostrar_Tazas();

                var itemsViewModel = new List<CartItemViewModel>();

                foreach (DataRow carrito in dtCarrito.Rows)
                {
                    foreach (DataRow tazas in dtTazas.Rows)
                    {
                        if (carrito[1].ToString() == tazas[0].ToString())
                        {
                            foreach (DataRow tamanos in dtTamanos.Rows)
                            {
                                if (carrito[2].ToString() == tamanos[0].ToString())
                                {
                                    double precioUnitario = Convert.ToDouble(tamanos[2]);
                                    int cantidad = carrito.Field<int>("cantidad");

                                    itemsViewModel.Add(new CartItemViewModel
                                    {
                                        // --- ¡AQUÍ ESTÁ LA CORRECCIÓN! ---
                                        // Usamos tazas[0] como nos indicaste.
                                        ProductId = Convert.ToInt32(tazas[0]),
                                        // ----------------------------------
                                        SizeId = Convert.ToInt32(carrito[2]),
                                        NombreProducto = tazas[1].ToString(),
                                        NombreTamano = tamanos[1].ToString(),
                                        RutaVideo = tazas["ruta_diseno"].ToString(),
                                        Cantidad = cantidad,
                                        PrecioUnitario = precioUnitario,
                                        PrecioTotalItem = precioUnitario * cantidad
                                    });
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }

                ViewBag.CartItems = itemsViewModel;
            }
            catch (Exception ex)
            {
                ViewBag.CartItems = new List<CartItemViewModel>();
                ViewBag.LoadError = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public IActionResult ActualizarCantidad(int productId, int sizeId, int cantidad)
        {
            if (!Cookies())
                return RedirectToAction("InicioSesion", "Home");
            Carrito carrito = new Carrito();
            carrito.Id_User = Sesion.Id;
            carrito.Id_Taza = productId;
            carrito.Id_Tamano = sizeId;
            carrito.Cantidad = cantidad;

            try
            {
                if (Home_SQL.Modificar_Carrito_Cantidad(carrito))
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return NotFound(new { success = false, message = "Artículo no encontrado." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        public IActionResult EliminarCarrito(int productId, int sizeId)
        {
            if (!Cookies())
                return RedirectToAction("InicioSesion", "Home");
            Carrito carrito = new Carrito();
            carrito.Id_User = Sesion.Id;
            carrito.Id_Taza = productId;
            carrito.Id_Tamano = sizeId;

            try
            {
                if (Home_SQL.Eliminar_Carrito(carrito))
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return NotFound(new { success = false, message = "Artículo no encontrado." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        public IActionResult DireccionPago(int carrito, Carrito item)
        {
            if (!Cookies())
                return RedirectToAction("InicioSesion", "Home");

            DataTable dt;
            if (carrito == 0)
            {
                dt = Home_SQL.Mostrar_Carrito(Sesion.Id);
                ViewBag.CartItems = dt;
            }
            else
            {
                item.Id_User = Sesion.Id;
                CarritoCompra.item = item;
                ViewBag.CarritoCompra = item;
            }

            dt = Admin_SQL.Mostrar_Tamanos_Tazas();
            ViewBag.Tamanos = dt;
            dt = Home_SQL.Mostrar_Tazas();
            ViewBag.Tazas = dt;
            ViewBag.Carrito = carrito;

            return View();
        }

        [HttpGet]
        public IActionResult Confirmacion(string Id_Pedido)
        {
            if (!Cookies())
                return RedirectToAction("InicioSesion", "Home");

            if (Sesion.compra == 0)
                return RedirectToAction("MisPedidos", "User");
            
            ViewBag.Id_Pedido = Id_Pedido;
            double subtotal, total = 0;
            
            DataTable dtCarrito = Home_SQL.Mostrar_Carrito(Sesion.Id);
            DataTable dtTaza = Admin_SQL.Mostrar_Tamanos_Tazas();

            foreach(DataRow carrito in dtCarrito.Rows)
            {
                Pedido pedido = new Pedido();
                Carrito carrito1 = new Carrito();

                pedido.Id_Pedido = Id_Pedido;

                pedido.Id_User = carrito[0].ToString()!;
                carrito1.Id_User = Convert.ToInt32(carrito[0].ToString()!);

                pedido.Id_Taza = carrito[1].ToString()!;
                carrito1.Id_Taza = Convert.ToInt32(carrito[1].ToString()!);

                pedido.Id_Tamano = carrito[2].ToString()!;
                carrito1.Id_Tamano = Convert.ToInt32(carrito[2].ToString()!);

                pedido.Cantidad = carrito[3].ToString()!;
                carrito1.Cantidad = Convert.ToInt32(carrito[3].ToString()!);

                pedido.Status = "En proceso";

                foreach(DataRow taza in dtTaza.Rows)
                {
                    if(carrito[2].ToString() == taza[0].ToString())
                    {
                        subtotal = Convert.ToDouble(carrito[3]) * Convert.ToDouble(taza[2]);
                        pedido.Precio = subtotal;
                        total += subtotal;
                        break;
                    }
                }

                Home_SQL.Agregar_Pedido_Item(pedido);

                Home_SQL.Eliminar_Carrito(carrito1);
            }

            Home_SQL.Agregar_Pedido(Id_Pedido, total);
            
            foreach(DataRow orden in Home_SQL.Mostrar_Pedido(Sesion.Id).Rows)
            {
                if(orden[1].ToString() == Id_Pedido)
                {
                    EnviarCorreoPedido(orden);
                    break;
                }
            }

            Sesion.compra = 0;
            return View();
        }
        [HttpGet]
        public IActionResult CrearIdPedido()
        {
            if (!Cookies())
                return RedirectToAction("InicioSesion", "Home");

            if(Home_SQL.Contar_Items_Carrito() <= 0)
                return RedirectToAction("InicioSesion", "Home");
                
            string Id_Pedido = "PED-";
            Id_Pedido += DateTime.Now.ToString("yyMMddHHmmss");
            Id_Pedido += "-" + Sesion.Id;
            Sesion.compra = 1;

            return RedirectToAction("Confirmacion", "Home", new { Id_Pedido = Id_Pedido });
        }
        public void EnviarCorreoPedido(DataRow orden)
        {
            DataTable tamanos = Home_SQL.Mostrar_Tazas();
            DataTable tazas = Admin_SQL.Mostrar_Tamanos_Tazas();
            DataTable items = Home_SQL.Mostrar_Pedido_Items();
            string correoUsuario = Sesion.Email; 

            try
            {
                // 1. Configuración del cliente SMTP (Usa los datos de tu servidor o Gmail)
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("ricardo7772005@gmail.com", "cmya ducv jfzq bvqt"),
                    EnableSsl = true,
                };

                // 2. Construcción del cuerpo del correo en HTML
                StringBuilder sb = new StringBuilder();
                sb.Append("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>");
                sb.Append($"<h2 style='color: #333;'>¡Gracias por tu pedido, #{orden[1]}!</h2>");
                sb.Append($"<p>Fecha: {Convert.ToDateTime(orden[5]):dd/MM/yyyy HH:mm}</p>");
                sb.Append("<table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>");
                sb.Append("<thead style='background-color: #f8f9fa;'><tr>");
                sb.Append("<th style='padding: 10px; border-bottom: 1px solid #ddd; text-align: left;'>Producto</th>");
                sb.Append("<th style='padding: 10px; border-bottom: 1px solid #ddd; text-align: center;'>Cant.</th>");
                sb.Append("<th style='padding: 10px; border-bottom: 1px solid #ddd; text-align: right;'>Subtotal</th>");
                sb.Append("</tr></thead><tbody>");

                foreach (DataRow pedido in items.Rows)
                {
                    if (orden[1].ToString() == pedido[1].ToString())
                    {
                        // Buscamos los detalles (Misma lógica que en la vista)
                        var taza = tazas.AsEnumerable().FirstOrDefault(t => (int)t[0] == (int)pedido[2]);
                        var tamano = tamanos.AsEnumerable().FirstOrDefault(tt => (int)tt[0] == (int)pedido[3]);

                        if (taza != null && tamano != null)
                        {
                            double subtotal = Convert.ToDouble(tamano[2]) * Convert.ToDouble(pedido[5]);
                            sb.Append("<tr>");
                            sb.Append($"<td style='padding: 10px; border-bottom: 1px solid #eee;'><strong>{taza[1]}</strong><br><small>{tamano[1]}</small></td>");
                            sb.Append($"<td style='padding: 10px; border-bottom: 1px solid #eee; text-align: center;'>{pedido[5]}</td>");
                            sb.Append($"<td style='padding: 10px; border-bottom: 1px solid #eee; text-align: right;'>${subtotal:N2}</td>");
                            sb.Append("</tr>");
                        }
                    }
                }

                sb.Append("</tbody></table>");
                sb.Append($"<div style='text-align: right; margin-top: 20px;'><h3>Total Pagado: ${Convert.ToDouble(orden[3]):N2}</h3></div>");
                sb.Append("<p style='color: #777; font-size: 12px; margin-top: 30px;'>Este es un correo automático de Tazuki. No respondas a este mensaje.</p>");
                sb.Append("</div>");

                // 3. Configuración del mensaje
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("tu-correo@gmail.com", "Tazuki Tienda"),
                    Subject = $"Confirmación de Pedido #{orden[1]}",
                    Body = sb.ToString(),
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(correoUsuario);

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // Loguear error
                Console.WriteLine("Error al enviar correo: " + ex.Message);
            }
        }


        public bool Cookies()
        {
            var miCookie = HttpContext.Request.Cookies["Tazuky2"];
            if (Home_SQL.ComprobarCookie(miCookie!))
            {
                ViewBag.Activo = "1";
                ViewBag.Nombre = Sesion.Nombre;
                ViewBag.rol = Sesion.rol;
                return true;
            }
            return false;
        }
        public IActionResult CerrarSesion()
        {
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddDays(-1);
            options.Path = "/";
            HttpContext.Response.Cookies.Append("Tazuky2", "", options);

            return RedirectToAction("Index", "Home");
        }
        public IActionResult ErrorUsuario()
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
