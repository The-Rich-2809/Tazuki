using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tazuki.Models
{
    public class Admin_SQL : Controller
    {
        public static DataTable Mostrar_Tazas()
        {
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * From disenos";

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
        public static DataTable Mostrar_Tags()
        {
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * From tags";

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

        public static DataTable Agregar_Diseno()
        {
            DataTable dt = new DataTable();
            string sql;

            sql = "INSERT INTO disenos (nombre, tamano_taza, descripcion, ruta_diseno, fecha_creacion) VALUES (@nombre, @tamano_taza, @descripcion, @ruta_diseno, @fecha_creacion);";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("nombre", Datos.Nombre);
                comando.Parameters.AddWithValue("tamano_taza", Datos.tamanoTaza);
                comando.Parameters.AddWithValue("descripcion", Datos.descripcion);
                comando.Parameters.AddWithValue("ruta_diseno", Datos.rutaDiseno);
                comando.Parameters.AddWithValue("fecha_creacion", DateTime.Now);
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
