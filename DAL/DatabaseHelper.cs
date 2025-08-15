using System.Configuration;
using System.Data.SqlClient;

namespace SistemaCotizaciones.DAL
{
    public class DatabaseHelper
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["SistemaCotizacionesConnection"].ConnectionString;
            }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        // Método para probar la conexión
        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}