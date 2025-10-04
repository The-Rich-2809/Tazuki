using MySql.Data.MySqlClient;
using System.Data;

namespace Tazuki.Models
{
    public class Home_SQL
    {
        public static DataTable Mostrar_Tazas()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * From disenos Where publicado = 1";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                //comando.Parameters.AddWithValue("AccesoSite", Datos.AccesoSite);
                dt.Load(comando.ExecuteReader());
                conexionBD.Close();

            }
            catch (MySqlException ex)
            {
                Datos.Mensaje = "Error al buscar " + ex.Message;
                conexionBD.Close();
            }
            return dt;
        }
    }
}
