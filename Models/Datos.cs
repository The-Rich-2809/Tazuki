namespace Tazuki.Models
{
    public static class Datos
    {
        public static string Mensaje { get; set; }

        public static int Id { get; set; }
        public static string Nombre { get; set; }
        public static string tamanoTaza { get; set; }
        public static string[] tags { get; set; }
        public static string descripcion { get; set; }
        public static double precio { get; set; }
        public static string rutaDiseno { get; set; }
    }
    public class EtiquetaParaRecibir
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
