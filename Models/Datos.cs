namespace Tazuki.Models
{
    public static class Datos
    {
        public static string Mensaje { get; set; } = string.Empty;
        public static int Id { get; set; }
        public static string Nombre { get; set; } = string.Empty;
        public static string tamanoTaza { get; set; } = string.Empty;
        public static string[] tags { get; set; } = Array.Empty<string>();
        public static string descripcion { get; set; } = string.Empty;
        public static double precio { get; set; }
        public static string rutaDiseno { get; set; } = string.Empty;
        public static string[,] Etiquetas { get; set; }
        public static List<string[]> TagsList { get; set; } = new List<string[]>();
        public static string UploadToken { get; set; } = string.Empty;
        public static string UploadExt { get; set; } = string.Empty;
        public static int Cantidad { get; set; }
    }
    public class EtiquetaParaRecibir
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public double Precio { get; set; }
        public bool Activo { get; set; }
    }
    public class Tag
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
    public class User
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string pri_ape { get; set; } = string.Empty;
        public string seg_ape { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string password2 { get; set; } = string.Empty;
    }
    public static class Sesion
    {
        public static int Id { get; set; }
        public static string Email { get; set; } = string.Empty;
        public static string Nombre { get; set; } = string.Empty;
        public static string rol { get; set; } = string.Empty;
        public static int compra { get; set; }
        
    }
    public class Carrito
    {
        public int Id_User { get; set; }
        public int Id_Taza { get; set; }
        public int Id_Tamano { get; set; }
        public int Cantidad { get; set; }
    }
    public class Pedido
    {
        public string Id_Pedido { get; set; } = string.Empty;
        public string Id_User { get; set; } = string.Empty;
        public string Id_Taza { get; set; } = string.Empty;
        public string Id_Tamano { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Cantidad { get; set; } = string.Empty;
        public double Precio { get; set; }
    }
    public static class CarritoCompra
    {
        public static Carrito item { get; set; } = new Carrito();
    }
    
    // Plantilla para enviar los datos al frontend
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string NombreTamano { get; set; } = string.Empty;
        public string RutaVideo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double PrecioTotalItem { get; set; }
        public string PedidoId { get; set; } = string.Empty;
    }
}
