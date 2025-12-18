using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazuki.Models
{
    internal class Conexion
    {
        public static MySqlConnection conexion()
        {
            string cadenaConexion = "";
            //Prueba
            cadenaConexion = "datasource=192.168.101.16; username=rich; password=300920; database=disenos_db_prueba";
            //Servidor
            //cadenaConexion = "datasource=192.168.101.16; username=rich; password=300920; database=disenos_db";
            //cadenaConexion = "datasource=localhost; username=root; password=300920; database=disenos";

            try
            {
                MySqlConnection conexionBD = new MySqlConnection(cadenaConexion);

                return conexionBD;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
    }
}
