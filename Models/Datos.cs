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
    }
    public class EtiquetaParaRecibir
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
    public class Tag
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
