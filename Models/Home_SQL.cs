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

        public static (DataTable tazas, DataTable tags, DataTable disenoTags) Mostrar_Datos_Catalogo()
        {
            DataTable dtTazas = new DataTable();
            DataTable dtTags = new DataTable();
            DataTable dtDisenoTags = new DataTable();

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();
            try
            {
                MySqlCommand cmd1 = new MySqlCommand("SELECT * FROM disenos WHERE publicado = 1", conexionBD);
                dtTazas.Load(cmd1.ExecuteReader());

                MySqlCommand cmd2 = new MySqlCommand("SELECT * FROM tags", conexionBD);
                dtTags.Load(cmd2.ExecuteReader());

                MySqlCommand cmd3 = new MySqlCommand("SELECT * FROM diseno_tags", conexionBD);
                dtDisenoTags.Load(cmd3.ExecuteReader());
            }
            catch (MySqlException ex)
            {
                Datos.Mensaje = "Error al buscar " + ex.Message;
            }
            finally
            {
                conexionBD.Close();
            }
            return (dtTazas, dtTags, dtDisenoTags);
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

        public static int Contar_Items_Carrito()
        {
            int total = 0;
            // Consulta para contar cuántos items tiene un usuario específico
            string sql = "SELECT COUNT(*) FROM carrito_items WHERE usuario_id = @id_user;";

            MySqlConnection conexionBD = Conexion.conexion();
            
            try
            {
                conexionBD.Open();
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("@id_user", Sesion.Id);

                // ExecuteScalar devuelve la primera columna de la primera fila (el resultado del COUNT)
                total = Convert.ToInt32(comando.ExecuteScalar());
            }
            catch (MySqlException ex)
            {
                Datos.Mensaje = "Error al contar registros: " + ex.Message;
            }
            finally
            {
                // El bloque finally asegura que la conexión se cierre incluso si hay un error
                if (conexionBD.State == System.Data.ConnectionState.Open)
                {
                    conexionBD.Close();
                }
            }

            return total;
        }

        public static DataTable Mostrar_Pedido(int id_user)
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * From pedidos WHERE usuario_id = @id_user";

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

        public static DataTable Mostrar_Pedido_Items()
        {
            Datos.Mensaje = "";
            DataTable dt = new DataTable();
            string sql;

            sql = "SELECT * From pedido_items";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
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

        public static bool Agregar_Pedido_Item(Pedido pedido)
        {
            Datos.Mensaje = "";

            string sql = "INSERT INTO pedido_items (pedido_id, diseno_id, tamano_taza_id, precio, cantidad) VALUES (@id_pedido, @id_taza, @id_tamano, @precio, @cantidad);";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();
            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("id_pedido", pedido.Id_Pedido);
                comando.Parameters.AddWithValue("id_taza", pedido.Id_Taza);
                comando.Parameters.AddWithValue("id_tamano", pedido.Id_Tamano);
                comando.Parameters.AddWithValue("precio", pedido.Precio);
                comando.Parameters.AddWithValue("cantidad", pedido.Cantidad);

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
        public static bool Agregar_Pedido(string pedido, double precio)
        {
            Datos.Mensaje = "";

            string sql = "INSERT INTO pedidos (folio_pedido, usuario_id, monto_total, estatus, fecha_pedido) VALUES (@folio_pedido, @usuario_id, @monto_total, @estatus, @fecha_pedido);";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();
            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("folio_pedido", pedido);
                comando.Parameters.AddWithValue("usuario_id", Sesion.Id);
                comando.Parameters.AddWithValue("monto_total", precio);
                comando.Parameters.AddWithValue("estatus", "pendiente");
                comando.Parameters.AddWithValue("fecha_pedido", DateTime.Now);
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


        public static DataTable Mostrar_Tazas_Relacionadas(string id)
        {
            DataTable dt = new DataTable();
            string sql = @"
                SELECT d.*,
                       matched.tag_matches,
                       (SELECT GROUP_CONCAT(t2.nombre ORDER BY t2.nombre SEPARATOR '|')
                        FROM diseno_tags dt3
                        JOIN tags t2 ON dt3.tag_id = t2.id
                        WHERE dt3.diseno_id = d.id) AS tags_nombres
                FROM disenos d
                JOIN (
                    SELECT diseno_id, COUNT(*) AS tag_matches
                    FROM diseno_tags
                    WHERE tag_id IN (SELECT tag_id FROM diseno_tags WHERE diseno_id = @id1)
                    GROUP BY diseno_id
                ) AS matched ON d.id = matched.diseno_id
                WHERE d.id != @id2
                  AND d.publicado = 1
                ORDER BY RAND()
                LIMIT 20";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();
            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("id1", id);
                comando.Parameters.AddWithValue("id2", id);
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

        public static DataTable Mostrar_Tazas_Aleatorias(int cantidad)
        {
            DataTable dt = new DataTable();
            string sql = @"
                SELECT d.id, d.nombre, d.precio, d.ruta_diseno, d.modelo,
                       (SELECT GROUP_CONCAT(t.nombre ORDER BY t.nombre SEPARATOR '|')
                        FROM diseno_tags dt2
                        JOIN tags t ON dt2.tag_id = t.id
                        WHERE dt2.diseno_id = d.id) AS tags_nombres
                FROM disenos d
                WHERE d.publicado = 1
                ORDER BY RAND()
                LIMIT @cantidad";
            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, conexionBD);
                cmd.Parameters.AddWithValue("cantidad", cantidad);
                dt.Load(cmd.ExecuteReader());
            }
            catch (MySqlException ex) { Datos.Mensaje = "Error: " + ex.Message; }
            finally { conexionBD.Close(); }
            return dt;
        }

        public static (DataTable secciones, DataTable disenos) Mostrar_Secciones_Home()
        {
            DataTable dtSecciones = new DataTable();
            DataTable dtDisenos = new DataTable();
            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();
            try
            {
                MySqlCommand cmd1 = new MySqlCommand(
                    "SELECT * FROM secciones_home WHERE activo = 1 ORDER BY orden ASC, id ASC", conexionBD);
                dtSecciones.Load(cmd1.ExecuteReader());

                string sql2 = @"
                    SELECT shd.seccion_id, d.id AS diseno_id, d.nombre, d.precio,
                           d.ruta_diseno, d.modelo,
                           (SELECT GROUP_CONCAT(t.nombre ORDER BY t.nombre SEPARATOR '|')
                            FROM diseno_tags dt2
                            JOIN tags t ON dt2.tag_id = t.id
                            WHERE dt2.diseno_id = d.id) AS tags_nombres
                    FROM seccion_home_disenos shd
                    JOIN disenos d ON shd.diseno_id = d.id
                    JOIN secciones_home sh ON shd.seccion_id = sh.id
                    WHERE sh.activo = 1 AND d.publicado = 1
                    ORDER BY sh.orden ASC, sh.id ASC, shd.orden ASC";
                MySqlCommand cmd2 = new MySqlCommand(sql2, conexionBD);
                dtDisenos.Load(cmd2.ExecuteReader());
            }
            catch (MySqlException ex) { Datos.Mensaje = "Error: " + ex.Message; }
            finally { conexionBD.Close(); }
            return (dtSecciones, dtDisenos);
        }

        public static bool ComprobarCookie(string miCookie)
        {
            if (string.IsNullOrEmpty(miCookie))
                return false;

            string sql = "SELECT id, nombre, pri_apellido, seg_apellido, email, rol FROM usuarios WHERE email = @email LIMIT 1";
            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();
            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                comando.Parameters.AddWithValue("email", miCookie);
                MySqlDataReader reader = comando.ExecuteReader();
                if (reader.HasRows && reader.Read())
                {
                    Sesion.Id = reader.GetInt32("id");
                    Sesion.Email = reader.GetString("email");
                    Sesion.Nombre = reader.GetString("nombre") + " " + reader.GetString("pri_apellido") + " " + reader.GetString("seg_apellido");
                    Sesion.rol = reader.GetString("rol");
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

    }


}
