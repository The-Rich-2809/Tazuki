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

            sql = "SELECT * FROM tags ORDER BY id ASC";

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
        public static DataTable Buscar_Tags(string nombre)
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * FROM tags WHERE nombre = @nombre";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("nombre", nombre);
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
        public static DataTable Mostrar_Tazas_Tags_Id(string id)
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * From diseno_tags Where diseno_id = @id ORDER BY diseno_id ASC";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("id", id);
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
                   (nombre, precio, tamano_taza_id, descripcion, ruta_diseno, fecha_creacion, publicado) 
                   VALUES (@nombre, @precio, @tamano_taza_id, @descripcion, @ruta_diseno, @fecha_creacion, @publicado);";

            using (MySqlConnection conexionBD = Conexion.conexion())
            {
                conexionBD.Open();

                try
                {
                    using (MySqlCommand comando = new MySqlCommand(sql, conexionBD))
                    {
                        comando.Parameters.AddWithValue("@nombre", Datos.Nombre);
                        comando.Parameters.AddWithValue("@precio", Datos.precio);
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
        public static int Agregar_Tags()
        {
            Datos.Mensaje = "";
            int nuevoId = 0;
            string sql = "INSERT INTO tags (nombre) VALUES (@nombre);";

            using (MySqlConnection conexionBD = Conexion.conexion())
            {
                try
                {
                    conexionBD.Open();

                    using (MySqlCommand comando = new MySqlCommand(sql, conexionBD))
                    {
                        comando.Parameters.AddWithValue("@nombre", Datos.Nombre);
                        comando.ExecuteNonQuery(); // ejecuta el INSERT

                        // Recuperar el ID insertado
                        nuevoId = (int)comando.LastInsertedId;
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062)
                        Datos.Mensaje = "La etiqueta ya existe.";
                    else
                        Datos.Mensaje = "Error al insertar: " + ex.Message;
                }
            }

            return nuevoId;
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
        public static bool Mod_ActDesDiseno(int i)
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "UPDATE disenos SET publicado = @publicado WHERE id = @id;";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("publicado", i);
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
        public static DataTable Eliminar_Diseno()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "DELETE FROM disenos WHERE id = @id";

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
        public static DataTable Eliminar_Diseno_Tags()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "DELETE FROM diseno_tags WHERE diseno_id = @diseno_id";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("diseno_id", Datos.Id);
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
