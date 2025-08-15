using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SistemaCotizaciones.Models;

namespace SistemaCotizaciones.DAL
{
    public class ClienteDAL
    {
        public bool RegistrarCliente(Cliente cliente)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    // Verificar si ya existe
                    string checkQuery = "SELECT COUNT(*) FROM Clientes WHERE Identificacion = @Identificacion";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Identificacion", cliente.Identificacion);

                    conn.Open();
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0) return false; // Cliente ya existe

                    string insertQuery = @"INSERT INTO Clientes (Nombre, Identificacion, Telefono, Email, Direccion, TipoCliente) 
                                         VALUES (@Nombre, @Identificacion, @Telefono, @Email, @Direccion, @TipoCliente)";

                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    insertCmd.Parameters.AddWithValue("@Identificacion", cliente.Identificacion);
                    insertCmd.Parameters.AddWithValue("@Telefono", cliente.Telefono ?? (object)DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@Email", cliente.Email ?? (object)DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@Direccion", cliente.Direccion ?? (object)DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@TipoCliente", cliente.TipoCliente ?? (object)DBNull.Value);

                    return insertCmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public Cliente BuscarClientePorIdentificacion(string identificacion)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT * FROM Clientes WHERE Identificacion = @Identificacion AND Estado = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Identificacion", identificacion);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return new Cliente
                        {
                            ID = (int)reader["ID"],
                            Nombre = reader["Nombre"].ToString(),
                            Identificacion = reader["Identificacion"].ToString(),
                            Telefono = reader["Telefono"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            Direccion = reader["Direccion"]?.ToString(),
                            TipoCliente = reader["TipoCliente"]?.ToString(),
                            Estado = (bool)reader["Estado"],
                            FechaCreacion = (DateTime)reader["FechaCreacion"]
                        };
                    }
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public bool EditarCliente(Cliente cliente)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE Clientes SET 
                                   Nombre = @Nombre, 
                                   Telefono = @Telefono, 
                                   Email = @Email, 
                                   Direccion = @Direccion, 
                                   TipoCliente = @TipoCliente,
                                   UltimaActividad = GETDATE()
                                   WHERE ID = @ID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", cliente.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoCliente", cliente.TipoCliente ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ID", cliente.ID);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool EliminarCliente(int clienteId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "UPDATE Clientes SET Estado = 0 WHERE ID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", clienteId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<Cliente> ObtenerTodosLosClientes()
        {
            List<Cliente> clientes = new List<Cliente>();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT * FROM Clientes WHERE Estado = 1 ORDER BY Nombre";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        clientes.Add(new Cliente
                        {
                            ID = (int)reader["ID"],
                            Nombre = reader["Nombre"].ToString(),
                            Identificacion = reader["Identificacion"].ToString(),
                            Telefono = reader["Telefono"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            Direccion = reader["Direccion"]?.ToString(),
                            TipoCliente = reader["TipoCliente"]?.ToString(),
                            Estado = (bool)reader["Estado"],
                            FechaCreacion = (DateTime)reader["FechaCreacion"]
                        });
                    }
                }
            }
            catch
            {
                // Log error
            }
            return clientes;
        }
    }
}