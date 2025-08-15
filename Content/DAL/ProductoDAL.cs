using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SistemaCotizaciones.Models;

namespace SistemaCotizaciones.DAL
{
    public class ProductoDAL
    {
        public bool RegistrarProducto(Producto producto)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    // Verificar si ya existe por código Cabys
                    string checkQuery = "SELECT COUNT(*) FROM Productos WHERE CodigoCabys = @CodigoCabys";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@CodigoCabys", producto.CodigoCabys);

                    conn.Open();
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0) return false; // Producto ya existe

                    string insertQuery = @"INSERT INTO Productos (CodigoCabys, Nombre, Descripcion, CantidadDisponible, Precio) 
                                        VALUES (@CodigoCabys, @Nombre, @Descripcion, @CantidadDisponible, @Precio)";

                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@CodigoCabys", producto.CodigoCabys);
                    insertCmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    insertCmd.Parameters.AddWithValue("@Descripcion", producto.Descripcion ?? (object)DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@CantidadDisponible", producto.CantidadDisponible);
                    insertCmd.Parameters.AddWithValue("@Precio", producto.Precio);

                    return insertCmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public Producto BuscarProductoPorCodigo(string codigo)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT * FROM Productos WHERE (CodigoCabys = @Codigo OR ID = @ID) AND Estado = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Codigo", codigo);

                    // Intentar convertir a ID si es numérico
                    int id = 0;
                    if (int.TryParse(codigo, out id))
                        cmd.Parameters.AddWithValue("@ID", id);
                    else
                        cmd.Parameters.AddWithValue("@ID", 0);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return new Producto
                        {
                            ID = (int)reader["ID"],
                            CodigoCabys = reader["CodigoCabys"].ToString(),
                            Nombre = reader["Nombre"].ToString(),
                            Descripcion = reader["Descripcion"]?.ToString(),
                            CantidadDisponible = (int)reader["CantidadDisponible"],
                            Precio = (decimal)reader["Precio"],
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

        

        public Producto BuscarProductoPorCodigoCabys(string codigoCabys)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT * FROM Productos WHERE CodigoCabys = @CodigoCabys AND Estado = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CodigoCabys", codigoCabys);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return new Producto
                        {
                            ID = (int)reader["ID"],
                            CodigoCabys = reader["CodigoCabys"].ToString(),
                            Nombre = reader["Nombre"].ToString(),
                            Descripcion = reader["Descripcion"]?.ToString(),
                            CantidadDisponible = (int)reader["CantidadDisponible"],
                            Precio = (decimal)reader["Precio"],
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

        public Producto BuscarProductoPorID(int id)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT * FROM Productos WHERE ID = @ID AND Estado = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return new Producto
                        {
                            ID = (int)reader["ID"],
                            CodigoCabys = reader["CodigoCabys"].ToString(),
                            Nombre = reader["Nombre"].ToString(),
                            Descripcion = reader["Descripcion"]?.ToString(),
                            CantidadDisponible = (int)reader["CantidadDisponible"],
                            Precio = (decimal)reader["Precio"],
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

        public bool EditarProducto(Producto producto)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE Productos SET 
                                  Nombre = @Nombre, 
                                  Descripcion = @Descripcion, 
                                  CantidadDisponible = @CantidadDisponible, 
                                  Precio = @Precio
                                  WHERE ID = @ID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", producto.Descripcion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CantidadDisponible", producto.CantidadDisponible);
                    cmd.Parameters.AddWithValue("@Precio", producto.Precio);
                    cmd.Parameters.AddWithValue("@ID", producto.ID);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool EliminarProducto(int productoId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "UPDATE Productos SET Estado = 0 WHERE ID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", productoId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<Producto> ObtenerTodosLosProductos()
        {
            List<Producto> productos = new List<Producto>();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT * FROM Productos WHERE Estado = 1 ORDER BY Nombre";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        productos.Add(new Producto
                        {
                            ID = (int)reader["ID"],
                            CodigoCabys = reader["CodigoCabys"].ToString(),
                            Nombre = reader["Nombre"].ToString(),
                            Descripcion = reader["Descripcion"]?.ToString(),
                            CantidadDisponible = (int)reader["CantidadDisponible"],
                            Precio = (decimal)reader["Precio"],
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
            return productos;
        }

        public bool ActualizarStock(int productoId, int cantidadVendida)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "UPDATE Productos SET CantidadDisponible = CantidadDisponible - @Cantidad WHERE ID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Cantidad", cantidadVendida);
                    cmd.Parameters.AddWithValue("@ID", productoId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool ValidarStock(int productoId, int cantidadSolicitada)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT CantidadDisponible FROM Productos WHERE ID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", productoId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int stockDisponible = (int)result;
                        return stockDisponible >= cantidadSolicitada;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}