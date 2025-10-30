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
        public static DataTable Mostrar_Users()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * From usuarios";

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
        public static DataTable Mostrar_Carrito(int id_user)
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * From carrito_items WHERE usuario_id = @id_user";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("id_user", id_user);
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
        public static bool Ingresar(User user)
        {
            MySqlDataReader reader;
            string sql = "SELECT * From usuarios WHERE email = @email AND password = @password LIMIT 1";
            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("email", user.email);
                comando.Parameters.AddWithValue("password", user.password);
                reader = comando.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Sesion.Id = reader.GetInt32(0);
                        Sesion.Email = reader.GetString(4);
                    }

                    return true;
                }
            }
            catch (MySqlException ex)
            {
                Datos.Mensaje = "Error al buscar " + ex.Message;
            }
            finally
            {
                conexionBD.Close();
            }
            return false;
        }
        public static bool Buscar_User(string email)
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * FROM usuarios WHERE email = @email";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("email", email);
                dt.Load(comando.ExecuteReader());
                conexionBD.Close();
                if (dt.Rows.Count == 0)
                    return true;
                else
                {
                    Datos.Mensaje = "El correo electrónico ya está registrado.";
                    return false;
                }
            }
            catch (MySqlException ex)
            {
                Datos.Mensaje = "Error al buscar " + ex.Message;
                conexionBD.Close();
                return false;
            }
        }
        public static bool Agregar_User(User user)
        {
            Datos.Mensaje = "";

            string sql = "INSERT INTO usuarios (nombre, pri_apellido, seg_apellido, email, password, rol) VALUES (@nombre, @pri_ape, @seg_ape, @email, @password, @rol);";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();
            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("nombre", user.nombre);
                comando.Parameters.AddWithValue("pri_ape", user.pri_ape);
                comando.Parameters.AddWithValue("seg_ape", user.seg_ape);
                comando.Parameters.AddWithValue("email", user.email);
                comando.Parameters.AddWithValue("password", user.password);
                comando.Parameters.AddWithValue("rol", "usuario");
                comando.ExecuteNonQuery();
                conexionBD.Close();
                return true;

            }
            catch (MySqlException ex)
            {
                Datos.Mensaje = "Error al insertar " + ex.Message;
                conexionBD.Close();
                return false;
            }
        }
        
        public static bool Comprobar_Carrito(Carrito carrito)
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * FROM carrito_items WHERE usuario_id = @id_user AND diseno_id = @id_taza AND tamano_taza_id = @id_tamano";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("id_user", carrito.Id_User);
                comando.Parameters.AddWithValue("id_taza", carrito.Id_Taza);
                comando.Parameters.AddWithValue("id_tamano", carrito.Id_Tamano);
                dt.Load(comando.ExecuteReader());
                conexionBD.Close();
                if (dt.Rows.Count > 0)
                {
                    Datos.Cantidad = Convert.ToInt32(dt.Rows[0]["cantidad"]);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MySqlException ex)
            {
                Datos.Mensaje = "Error al buscar " + ex.Message;
                conexionBD.Close();
                return false;
            }
        }
        public static bool Agregar_Carrito(Carrito carrito)
        {
            Datos.Mensaje = "";

            string sql = "INSERT INTO carrito_items (usuario_id, diseno_id, tamano_taza_id, cantidad, fecha_agregado) VALUES (@id_user, @id_taza, @id_tamano, @cantidad, @fecha_agregado);";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();
            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("id_user", carrito.Id_User);
                comando.Parameters.AddWithValue("id_taza", carrito.Id_Taza);
                comando.Parameters.AddWithValue("id_tamano", carrito.Id_Tamano);
                comando.Parameters.AddWithValue("cantidad", carrito.Cantidad);
                comando.Parameters.AddWithValue("fecha_agregado", DateTime.Now);
                comando.ExecuteNonQuery();
                conexionBD.Close();
                return true;

            }
            catch (MySqlException ex)
            {
                Datos.Mensaje = "Error al insertar " + ex.Message;
                conexionBD.Close();
                return false;
            }
        }
        public static bool Modificar_Carrito_Cantidad(Carrito carrito)
        {
            Datos.Mensaje = "";

            string sql = "UPDATE carrito_items SET cantidad = @cantidad WHERE usuario_id = @id_user AND diseno_id = @id_taza AND tamano_taza_id = @id_tamano;";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();
            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("id_user", carrito.Id_User);
                comando.Parameters.AddWithValue("id_taza", carrito.Id_Taza);
                comando.Parameters.AddWithValue("id_tamano", carrito.Id_Tamano);
                comando.Parameters.AddWithValue("cantidad", carrito.Cantidad);
                comando.ExecuteNonQuery();
                conexionBD.Close();
                return true;

            }
            catch (MySqlException ex)
            {
                Datos.Mensaje = "Error al modificar " + ex.Message;
                conexionBD.Close();
                return false;
            }
        }
        public static bool Eliminar_Carrito(Carrito carrito)
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "DELETE FROM carrito_items WHERE usuario_id = @id_user AND diseno_id = @id_taza AND tamano_taza_id = @id_tamano";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("id_user", carrito.Id_User);
                comando.Parameters.AddWithValue("id_taza", carrito.Id_Taza);
                comando.Parameters.AddWithValue("id_tamano", carrito.Id_Tamano);
                dt.Load(comando.ExecuteReader());
                conexionBD.Close();
                return true;

            }
            catch (MySqlException ex)
            {
                Datos.Mensaje = "Error al buscar " + ex.Message;
                conexionBD.Close();
                return false;
            }
        }

        public static bool ComprobarCookie(string miCookie)
        {
            DataTable data = Mostrar_Users();
            foreach (DataRow row in data.Rows)
            {
                if (miCookie == row.Field<string>("email"))
                {
                    Sesion.Id = row.Field<Int32>("id");
                    Sesion.Email = row.Field<string>("email");
                    Sesion.Nombre = row.Field<string>("nombre") + " " + row.Field<string>("pri_apellido") + " " + row.Field<string>("seg_apellido");
                    Sesion.rol = row.Field<string>("rol");
                    return true;
                }
            }
            return false;
        }

    }


}
