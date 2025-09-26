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
            cadenaConexion = "datasource=192.168.101.4; username=rich; password=TheRich2809; database=disenos_db";
            //cadenaConexion = "Server=MYSQL1002.site4now.net;Database=db_ab867f_check;Uid=ab867f_check;Pwd=Intervalo2024";

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
