using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SistemaCotizaciones.Models;

namespace SistemaCotizaciones.DAL
{
    public class CotizacionDAL
    {
        private const decimal TASA_IVA = 0.13m; // 13% IVA en Costa Rica

        public bool CrearCotizacion(Cotizacion cotizacion)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Verificar que el cliente existe y está activo
                            string checkClienteQuery = "SELECT COUNT(*) FROM Clientes WHERE ID = @ClienteID AND Estado = 1";
                            SqlCommand checkClienteCmd = new SqlCommand(checkClienteQuery, conn, transaction);
                            checkClienteCmd.Parameters.AddWithValue("@ClienteID", cotizacion.ClienteID);
                            int clienteCount = (int)checkClienteCmd.ExecuteScalar();

                            if (clienteCount == 0) return false; // Cliente no existe o inactivo

                            // Verificar que el producto existe y tiene stock suficiente
                            string checkProductoQuery = @"SELECT CantidadDisponible, Precio FROM Productos 
                                                       WHERE ID = @ProductoID AND Estado = 1";
                            SqlCommand checkProductoCmd = new SqlCommand(checkProductoQuery, conn, transaction);
                            checkProductoCmd.Parameters.AddWithValue("@ProductoID", cotizacion.ProductoID);
                            SqlDataReader reader = checkProductoCmd.ExecuteReader();

                            if (!reader.Read())
                            {
                                reader.Close();
                                return false; // Producto no existe
                            }

                            int stockDisponible = (int)reader["CantidadDisponible"];
                            decimal precioProducto = (decimal)reader["Precio"];
                            reader.Close();

                            if (stockDisponible < cotizacion.CantidadProducto)
                                return false; // Stock insuficiente

                            // Calcular automáticamente los valores
                            cotizacion.PrecioUnitario = precioProducto;
                            cotizacion.Bruto = cotizacion.CantidadProducto * cotizacion.PrecioUnitario;

                            // Validar que el descuento no sea mayor al bruto
                            if (cotizacion.Descuento > cotizacion.Bruto)
                                cotizacion.Descuento = 0;

                            decimal subtotal = cotizacion.Bruto - cotizacion.Descuento;
                            cotizacion.IVA = subtotal * TASA_IVA;
                            cotizacion.Total = subtotal + cotizacion.IVA;

                            // Insertar la cotización
                            string insertQuery = @"INSERT INTO Cotizaciones 
                                                (ClienteID, ProductoID, CantidadProducto, PrecioUnitario, 
                                                 Bruto, Descuento, IVA, Total, FechaVencimiento) 
                                                VALUES 
                                                (@ClienteID, @ProductoID, @CantidadProducto, @PrecioUnitario, 
                                                 @Bruto, @Descuento, @IVA, @Total, @FechaVencimiento)";

                            SqlCommand insertCmd = new SqlCommand(insertQuery, conn, transaction);
                            insertCmd.Parameters.AddWithValue("@ClienteID", cotizacion.ClienteID);
                            insertCmd.Parameters.AddWithValue("@ProductoID", cotizacion.ProductoID);
                            insertCmd.Parameters.AddWithValue("@CantidadProducto", cotizacion.CantidadProducto);
                            insertCmd.Parameters.AddWithValue("@PrecioUnitario", cotizacion.PrecioUnitario);
                            insertCmd.Parameters.AddWithValue("@Bruto", cotizacion.Bruto);
                            insertCmd.Parameters.AddWithValue("@Descuento", cotizacion.Descuento);
                            insertCmd.Parameters.AddWithValue("@IVA", cotizacion.IVA);
                            insertCmd.Parameters.AddWithValue("@Total", cotizacion.Total);
                            insertCmd.Parameters.AddWithValue("@FechaVencimiento",
                                cotizacion.FechaVencimiento ?? DateTime.Now.AddDays(30)); // 30 días por defecto

                            int result = insertCmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                // Actualizar última actividad del cliente
                                string updateClienteQuery = "UPDATE Clientes SET UltimaActividad = GETDATE() WHERE ID = @ClienteID";
                                SqlCommand updateClienteCmd = new SqlCommand(updateClienteQuery, conn, transaction);
                                updateClienteCmd.Parameters.AddWithValue("@ClienteID", cotizacion.ClienteID);
                                updateClienteCmd.ExecuteNonQuery();

                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public List<Cotizacion> ConsultarCotizacionesPorCliente(int clienteId)
        {
            List<Cotizacion> cotizaciones = new List<Cotizacion>();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"SELECT c.*, cl.Nombre as NombreCliente, p.Nombre as NombreProducto, p.CodigoCabys
                                  FROM Cotizaciones c
                                  INNER JOIN Clientes cl ON c.ClienteID = cl.ID
                                  INNER JOIN Productos p ON c.ProductoID = p.ID
                                  WHERE c.ClienteID = @ClienteID AND c.Estado = 'Activa'
                                  ORDER BY c.FechaCotizacion DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ClienteID", clienteId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cotizaciones.Add(new Cotizacion
                        {
                            ID = (int)reader["ID"],
                            ClienteID = (int)reader["ClienteID"],
                            FechaCotizacion = (DateTime)reader["FechaCotizacion"],
                            ProductoID = (int)reader["ProductoID"],
                            CantidadProducto = (int)reader["CantidadProducto"],
                            PrecioUnitario = (decimal)reader["PrecioUnitario"],
                            Bruto = (decimal)reader["Bruto"],
                            Descuento = (decimal)reader["Descuento"],
                            IVA = (decimal)reader["IVA"],
                            Total = (decimal)reader["Total"],
                            Estado = reader["Estado"].ToString(),
                            FechaVencimiento = reader["FechaVencimiento"] as DateTime?
                        });
                    }
                }
            }
            catch
            {
                // Log error
            }
            return cotizaciones;
        }

        public Cotizacion BuscarCotizacionPorId(int cotizacionId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"SELECT c.*, cl.Nombre as NombreCliente, cl.Identificacion,
                                  p.Nombre as NombreProducto, p.CodigoCabys
                                  FROM Cotizaciones c
                                  INNER JOIN Clientes cl ON c.ClienteID = cl.ID
                                  INNER JOIN Productos p ON c.ProductoID = p.ID
                                  WHERE c.ID = @ID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", cotizacionId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return new Cotizacion
                        {
                            ID = (int)reader["ID"],
                            ClienteID = (int)reader["ClienteID"],
                            FechaCotizacion = (DateTime)reader["FechaCotizacion"],
                            ProductoID = (int)reader["ProductoID"],
                            CantidadProducto = (int)reader["CantidadProducto"],
                            PrecioUnitario = (decimal)reader["PrecioUnitario"],
                            Bruto = (decimal)reader["Bruto"],
                            Descuento = (decimal)reader["Descuento"],
                            IVA = (decimal)reader["IVA"],
                            Total = (decimal)reader["Total"],
                            Estado = reader["Estado"].ToString(),
                            FechaVencimiento = reader["FechaVencimiento"] as DateTime?
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

        public bool EditarCotizacion(Cotizacion cotizacion)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Verificar stock del producto si cambió la cantidad
                            string checkStockQuery = "SELECT CantidadDisponible FROM Productos WHERE ID = @ProductoID";
                            SqlCommand checkStockCmd = new SqlCommand(checkStockQuery, conn, transaction);
                            checkStockCmd.Parameters.AddWithValue("@ProductoID", cotizacion.ProductoID);

                            object stockResult = checkStockCmd.ExecuteScalar();
                            if (stockResult == null) return false;

                            int stockDisponible = (int)stockResult;
                            if (stockDisponible < cotizacion.CantidadProducto) return false;

                            // Recalcular valores
                            cotizacion.Bruto = cotizacion.CantidadProducto * cotizacion.PrecioUnitario;

                            if (cotizacion.Descuento > cotizacion.Bruto)
                                cotizacion.Descuento = 0;

                            decimal subtotal = cotizacion.Bruto - cotizacion.Descuento;
                            cotizacion.IVA = subtotal * TASA_IVA;
                            cotizacion.Total = subtotal + cotizacion.IVA;

                            // Actualizar cotización
                            string updateQuery = @"UPDATE Cotizaciones SET 
                                                ProductoID = @ProductoID,
                                                CantidadProducto = @CantidadProducto,
                                                PrecioUnitario = @PrecioUnitario,
                                                Bruto = @Bruto,
                                                Descuento = @Descuento,
                                                IVA = @IVA,
                                                Total = @Total,
                                                FechaVencimiento = @FechaVencimiento
                                                WHERE ID = @ID";

                            SqlCommand updateCmd = new SqlCommand(updateQuery, conn, transaction);
                            updateCmd.Parameters.AddWithValue("@ProductoID", cotizacion.ProductoID);
                            updateCmd.Parameters.AddWithValue("@CantidadProducto", cotizacion.CantidadProducto);
                            updateCmd.Parameters.AddWithValue("@PrecioUnitario", cotizacion.PrecioUnitario);
                            updateCmd.Parameters.AddWithValue("@Bruto", cotizacion.Bruto);
                            updateCmd.Parameters.AddWithValue("@Descuento", cotizacion.Descuento);
                            updateCmd.Parameters.AddWithValue("@IVA", cotizacion.IVA);
                            updateCmd.Parameters.AddWithValue("@Total", cotizacion.Total);
                            updateCmd.Parameters.AddWithValue("@FechaVencimiento", cotizacion.FechaVencimiento);
                            updateCmd.Parameters.AddWithValue("@ID", cotizacion.ID);

                            int result = updateCmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public bool EliminarCotizacion(int cotizacionId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "UPDATE Cotizaciones SET Estado = 'Eliminada' WHERE ID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", cotizacionId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<Cotizacion> ObtenerTodasLasCotizaciones()
        {
            List<Cotizacion> cotizaciones = new List<Cotizacion>();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"SELECT c.*, cl.Nombre as NombreCliente, cl.Identificacion,
                                  p.Nombre as NombreProducto, p.CodigoCabys
                                  FROM Cotizaciones c
                                  INNER JOIN Clientes cl ON c.ClienteID = cl.ID
                                  INNER JOIN Productos p ON c.ProductoID = p.ID
                                  WHERE c.Estado = 'Activa'
                                  ORDER BY c.FechaCotizacion DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cotizaciones.Add(new Cotizacion
                        {
                            ID = (int)reader["ID"],
                            ClienteID = (int)reader["ClienteID"],
                            FechaCotizacion = (DateTime)reader["FechaCotizacion"],
                            ProductoID = (int)reader["ProductoID"],
                            CantidadProducto = (int)reader["CantidadProducto"],
                            PrecioUnitario = (decimal)reader["PrecioUnitario"],
                            Bruto = (decimal)reader["Bruto"],
                            Descuento = (decimal)reader["Descuento"],
                            IVA = (decimal)reader["IVA"],
                            Total = (decimal)reader["Total"],
                            Estado = reader["Estado"].ToString(),
                            FechaVencimiento = reader["FechaVencimiento"] as DateTime?
                        });
                    }
                }
            }
            catch
            {
                // Log error
            }
            return cotizaciones;
        }

        public List<Cotizacion> ObtenerCotizacionesPorVencer(int diasAnticipacion = 7)
        {
            List<Cotizacion> cotizaciones = new List<Cotizacion>();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"SELECT c.*, cl.Nombre as NombreCliente, cl.Identificacion,
                                  p.Nombre as NombreProducto, p.CodigoCabys
                                  FROM Cotizaciones c
                                  INNER JOIN Clientes cl ON c.ClienteID = cl.ID
                                  INNER JOIN Productos p ON c.ProductoID = p.ID
                                  WHERE c.Estado = 'Activa' 
                                  AND c.FechaVencimiento IS NOT NULL
                                  AND c.FechaVencimiento <= DATEADD(DAY, @DiasAnticipacion, GETDATE())
                                  AND c.FechaVencimiento >= GETDATE()
                                  ORDER BY c.FechaVencimiento";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@DiasAnticipacion", diasAnticipacion);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cotizaciones.Add(new Cotizacion
                        {
                            ID = (int)reader["ID"],
                            ClienteID = (int)reader["ClienteID"],
                            FechaCotizacion = (DateTime)reader["FechaCotizacion"],
                            ProductoID = (int)reader["ProductoID"],
                            CantidadProducto = (int)reader["CantidadProducto"],
                            PrecioUnitario = (decimal)reader["PrecioUnitario"],
                            Bruto = (decimal)reader["Bruto"],
                            Descuento = (decimal)reader["Descuento"],
                            IVA = (decimal)reader["IVA"],
                            Total = (decimal)reader["Total"],
                            Estado = reader["Estado"].ToString(),
                            FechaVencimiento = reader["FechaVencimiento"] as DateTime?
                        });
                    }
                }
            }
            catch
            {
                // Log error
            }
            return cotizaciones;
        }

        public decimal CalcularTotalVentasPorPeriodo(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"SELECT ISNULL(SUM(Total), 0) 
                                  FROM Cotizaciones 
                                  WHERE Estado = 'Vendida'
                                  AND FechaCotizacion BETWEEN @FechaInicio AND @FechaFin";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    conn.Open();
                    return (decimal)cmd.ExecuteScalar();
                }
            }
            catch
            {
                return 0;
            }
        }

        public bool MarcarComoVendida(int cotizacionId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Obtener información de la cotización
                            string getCotizacionQuery = "SELECT ProductoID, CantidadProducto FROM Cotizaciones WHERE ID = @ID";
                            SqlCommand getCotizacionCmd = new SqlCommand(getCotizacionQuery, conn, transaction);
                            getCotizacionCmd.Parameters.AddWithValue("@ID", cotizacionId);

                            SqlDataReader reader = getCotizacionCmd.ExecuteReader();
                            if (!reader.Read())
                            {
                                reader.Close();
                                transaction.Rollback();
                                return false;
                            }

                            int productoId = (int)reader["ProductoID"];
                            int cantidad = (int)reader["CantidadProducto"];
                            reader.Close();

                            // Actualizar stock del producto
                            ProductoDAL productoDAL = new ProductoDAL();
                            string updateStockQuery = "UPDATE Productos SET CantidadDisponible = CantidadDisponible - @Cantidad WHERE ID = @ID";
                            SqlCommand updateStockCmd = new SqlCommand(updateStockQuery, conn, transaction);
                            updateStockCmd.Parameters.AddWithValue("@Cantidad", cantidad);
                            updateStockCmd.Parameters.AddWithValue("@ID", productoId);
                            updateStockCmd.ExecuteNonQuery();

                            // Marcar cotización como vendida
                            string updateCotizacionQuery = "UPDATE Cotizaciones SET Estado = 'Vendida' WHERE ID = @ID";
                            SqlCommand updateCotizacionCmd = new SqlCommand(updateCotizacionQuery, conn, transaction);
                            updateCotizacionCmd.Parameters.AddWithValue("@ID", cotizacionId);

                            int result = updateCotizacionCmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // NUEVOS MÉTODOS AGREGADOS
        public List<CotizacionCompleta> ObtenerTodasLasCotizacionesCompletas()
        {
            List<CotizacionCompleta> cotizaciones = new List<CotizacionCompleta>();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"SELECT c.*, cl.Nombre as NombreCliente, cl.Identificacion as IdentificacionCliente,
                                  p.Nombre as NombreProducto, p.CodigoCabys, cl.TipoCliente,
                                  p.Descripcion as DescripcionProducto, p.CantidadDisponible as StockDisponible
                                  FROM Cotizaciones c
                                  INNER JOIN Clientes cl ON c.ClienteID = cl.ID
                                  INNER JOIN Productos p ON c.ProductoID = p.ID
                                  WHERE c.Estado = 'Activa'
                                  ORDER BY c.FechaCotizacion DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cotizaciones.Add(new CotizacionCompleta
                        {
                            ID = (int)reader["ID"],
                            ClienteID = (int)reader["ClienteID"],
                            NombreCliente = reader["NombreCliente"].ToString(),
                            IdentificacionCliente = reader["IdentificacionCliente"].ToString(),
                            FechaCotizacion = (DateTime)reader["FechaCotizacion"],
                            ProductoID = (int)reader["ProductoID"],
                            NombreProducto = reader["NombreProducto"].ToString(),
                            CodigoCabys = reader["CodigoCabys"].ToString(),
                            Cantidad = (int)reader["CantidadProducto"],
                            PrecioUnitario = (decimal)reader["PrecioUnitario"],
                            Bruto = (decimal)reader["Bruto"],
                            Descuento = (decimal)reader["Descuento"],
                            IVA = (decimal)reader["IVA"],
                            Total = (decimal)reader["Total"],
                            Estado = reader["Estado"].ToString(),
                            FechaVencimiento = reader["FechaVencimiento"] as DateTime?,
                            TipoCliente = reader["TipoCliente"]?.ToString(),
                            DescripcionProducto = reader["DescripcionProducto"]?.ToString(),
                            StockDisponible = (int)reader["StockDisponible"]
                        });
                    }
                }
            }
            catch
            {
                // Log error
            }
            return cotizaciones;
        }

        public CotizacionCompleta ObtenerCotizacionCompleta(int cotizacionId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"SELECT c.*, cl.Nombre as NombreCliente, cl.Identificacion as IdentificacionCliente,
                                  p.Nombre as NombreProducto, p.CodigoCabys, cl.TipoCliente,
                                  p.Descripcion as DescripcionProducto, p.CantidadDisponible as StockDisponible
                                  FROM Cotizaciones c
                                  INNER JOIN Clientes cl ON c.ClienteID = cl.ID
                                  INNER JOIN Productos p ON c.ProductoID = p.ID
                                  WHERE c.ID = @ID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", cotizacionId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return new CotizacionCompleta
                        {
                            ID = (int)reader["ID"],
                            ClienteID = (int)reader["ClienteID"],
                            NombreCliente = reader["NombreCliente"].ToString(),
                            IdentificacionCliente = reader["IdentificacionCliente"].ToString(),
                            FechaCotizacion = (DateTime)reader["FechaCotizacion"],
                            ProductoID = (int)reader["ProductoID"],
                            NombreProducto = reader["NombreProducto"].ToString(),
                            CodigoCabys = reader["CodigoCabys"].ToString(),
                            Cantidad = (int)reader["CantidadProducto"],
                            PrecioUnitario = (decimal)reader["PrecioUnitario"],
                            Bruto = (decimal)reader["Bruto"],
                            Descuento = (decimal)reader["Descuento"],
                            IVA = (decimal)reader["IVA"],
                            Total = (decimal)reader["Total"],
                            Estado = reader["Estado"].ToString(),
                            FechaVencimiento = reader["FechaVencimiento"] as DateTime?,
                            TipoCliente = reader["TipoCliente"]?.ToString(),
                            DescripcionProducto = reader["DescripcionProducto"]?.ToString(),
                            StockDisponible = (int)reader["StockDisponible"]
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

        public List<CotizacionCompleta> ObtenerCotizacionesPorClienteCompletas(int clienteId)
        {
            List<CotizacionCompleta> cotizaciones = new List<CotizacionCompleta>();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"SELECT c.*, cl.Nombre as NombreCliente, cl.Identificacion as IdentificacionCliente,
                                  p.Nombre as NombreProducto, p.CodigoCabys
                                  FROM Cotizaciones c
                                  INNER JOIN Clientes cl ON c.ClienteID = cl.ID
                                  INNER JOIN Productos p ON c.ProductoID = p.ID
                                  WHERE c.ClienteID = @ClienteID AND c.Estado = 'Activa'
                                  ORDER BY c.FechaCotizacion DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ClienteID", clienteId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cotizaciones.Add(new CotizacionCompleta
                        {
                            ID = (int)reader["ID"],
                            ClienteID = (int)reader["ClienteID"],
                            NombreCliente = reader["NombreCliente"].ToString(),
                            IdentificacionCliente = reader["IdentificacionCliente"].ToString(),
                            FechaCotizacion = (DateTime)reader["FechaCotizacion"],
                            ProductoID = (int)reader["ProductoID"],
                            NombreProducto = reader["NombreProducto"].ToString(),
                            CodigoCabys = reader["CodigoCabys"].ToString(),
                            Cantidad = (int)reader["CantidadProducto"],
                            PrecioUnitario = (decimal)reader["PrecioUnitario"],
                            Bruto = (decimal)reader["Bruto"],
                            Descuento = (decimal)reader["Descuento"],
                            IVA = (decimal)reader["IVA"],
                            Total = (decimal)reader["Total"],
                            Estado = reader["Estado"].ToString(),
                            FechaVencimiento = reader["FechaVencimiento"] as DateTime?
                        });
                    }
                }
            }
            catch
            {
                // Log error
            }
            return cotizaciones;
        }

        public bool EditarCotizacion(int cotizacionId, int cantidad, decimal descuento, decimal total)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    // Obtener precio unitario actual
                    string getPrecioQuery = "SELECT PrecioUnitario FROM Cotizaciones WHERE ID = @ID";
                    SqlCommand getPrecioCmd = new SqlCommand(getPrecioQuery, conn);
                    getPrecioCmd.Parameters.AddWithValue("@ID", cotizacionId);

                    conn.Open();
                    decimal precioUnitario = (decimal)getPrecioCmd.ExecuteScalar();

                    // Recalcular valores
                    decimal bruto = cantidad * precioUnitario;
                    decimal montoDescuento = (bruto * descuento) / 100;
                    decimal subtotal = bruto - montoDescuento;
                    decimal iva = subtotal * 0.13m;
                    decimal totalCalculado = subtotal + iva;

                    string updateQuery = @"UPDATE Cotizaciones SET 
                                        CantidadProducto = @Cantidad,
                                        Bruto = @Bruto,
                                        Descuento = @Descuento,
                                        IVA = @IVA,
                                        Total = @Total
                                        WHERE ID = @ID";

                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@Cantidad", cantidad);
                    updateCmd.Parameters.AddWithValue("@Bruto", bruto);
                    updateCmd.Parameters.AddWithValue("@Descuento", descuento);
                    updateCmd.Parameters.AddWithValue("@IVA", iva);
                    updateCmd.Parameters.AddWithValue("@Total", totalCalculado);
                    updateCmd.Parameters.AddWithValue("@ID", cotizacionId);

                    return updateCmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}