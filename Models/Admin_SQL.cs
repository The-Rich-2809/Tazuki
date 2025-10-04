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
            Datos.Mensaje = "";
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
            Datos.Mensaje = "";
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
        public static DataTable Mostrar_Tazas_Tags()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * From diseno_tags";

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
        public static DataTable Mostrar_Tamanos_Tazas()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * From tamanos_taza";

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

        public static bool Agregar_Diseno()
        {
            Datos.Mensaje = "";
            string sql = @"INSERT INTO disenos 
                   (nombre, tamano_taza_id, descripcion, ruta_diseno, fecha_creacion, publicado) 
                   VALUES (@nombre, @tamano_taza_id, @descripcion, @ruta_diseno, @fecha_creacion, @publicado);";

            using (MySqlConnection conexionBD = Conexion.conexion())
            {
                conexionBD.Open();

                try
                {
                    using (MySqlCommand comando = new MySqlCommand(sql, conexionBD))
                    {
                        comando.Parameters.AddWithValue("@nombre", Datos.Nombre);
                        comando.Parameters.AddWithValue("@tamano_taza_id", Datos.tamanoTaza);
                        comando.Parameters.AddWithValue("@descripcion", Datos.descripcion);
                        comando.Parameters.AddWithValue("@ruta_diseno", Datos.rutaDiseno);
                        comando.Parameters.AddWithValue("@fecha_creacion", DateTime.Now);
                        comando.Parameters.AddWithValue("@publicado", 1);

                        comando.ExecuteNonQuery();

                        // 👇 Aquí obtienes el ID autoincremental
                        long nuevoId = comando.LastInsertedId;
                        Datos.Id = (int)nuevoId;

                        return true;
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062)
                        Datos.Mensaje = "El diseño ya existe.";
                    else
                        Datos.Mensaje = "Error al insertar: " + ex.Message;

                    return false; // o 0, como prefieras
                }
            }
        }

        public static DataTable Agregar_Tags()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "INSERT INTO tags (nombre) VALUES (@nombre);";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("nombre", Datos.Nombre);
                dt.Load(comando.ExecuteReader());
                conexionBD.Close();

            }
            catch (MySqlException ex)
            {
                if(ex.Number == 1062)
                    Datos.Mensaje = "La etiqueta ya existe.";
                else
                    Datos.Mensaje = "Error al buscar " + ex.Message;

                conexionBD.Close();
            }
            return dt;
        }

        public static DataTable Agregar_Diseno_Tags(string idTag)
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "INSERT INTO diseno_tags (diseno_id, tag_id) VALUES (@diseno_id, @tag_id);";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("diseno_id", Datos.Id);
                comando.Parameters.AddWithValue("tag_id", idTag);
                dt.Load(comando.ExecuteReader());
                conexionBD.Close();

            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                    Datos.Mensaje = "La etiqueta ya existe.";
                else
                    Datos.Mensaje = "Error al buscar " + ex.Message;

                conexionBD.Close();
            }
            return dt;
        }
        public static DataTable Agregar_Tamano()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "INSERT INTO tamanos_taza (nombre) VALUES (@nombre);";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("nombre", Datos.Nombre);
                dt.Load(comando.ExecuteReader());
                conexionBD.Close();

            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                    Datos.Mensaje = "La etiqueta ya existe.";
                else
                    Datos.Mensaje = "Error al buscar " + ex.Message;

                conexionBD.Close();
            }
            return dt;
        }

        public static bool Mod_Tags()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "UPDATE tags SET nombre = @nombre WHERE id = @id;";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("nombre", Datos.Nombre);
                comando.Parameters.AddWithValue("id", Datos.Id);
                dt.Load(comando.ExecuteReader());
                conexionBD.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                    Datos.Mensaje = "La etiqueta ya existe.";
                else
                    Datos.Mensaje = "Error al buscar " + ex.Message;

                conexionBD.Close();
                return false;
            }
        }
        public static bool Mod_Tamano()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "UPDATE tamanos_taza SET nombre = @nombre WHERE id = @id;";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("nombre", Datos.Nombre);
                comando.Parameters.AddWithValue("id", Datos.Id);
                dt.Load(comando.ExecuteReader());
                conexionBD.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                    Datos.Mensaje = "El tamaño ya existe.";
                else
                    Datos.Mensaje = "Error al buscar " + ex.Message;

                conexionBD.Close();
                return false;
            }
        }

        public static DataTable Eliminar_Tags()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "DELETE FROM tags WHERE id = @id";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("id", Datos.Id);
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
        public static DataTable Eliminar_Tamano()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "DELETE FROM tamanos_taza WHERE id = @id";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("id", Datos.Id);
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
