using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using SistemaCotizaciones.DAL;
using SistemaCotizaciones.Models;

namespace SistemaCotizaciones.Pages
{
    public partial class Reportes : System.Web.UI.Page
    {
        private ClienteDAL clienteDAL = new ClienteDAL();
        private ProductoDAL productoDAL = new ProductoDAL();
        private CotizacionDAL cotizacionDAL = new CotizacionDAL();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar autenticación
            if (Session["Usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarEstadisticasGenerales();
            }
        }

        #region Estadísticas Generales

        protected void btnActualizarEstadisticas_Click(object sender, EventArgs e)
        {
            CargarEstadisticasGenerales();
        }

        private void CargarEstadisticasGenerales()
        {
            try
            {
                // Obtener estadísticas de clientes
                List<Cliente> clientes = clienteDAL.ObtenerTodosLosClientes();
                lblTotalClientes.Text = clientes.Count.ToString();

                // Obtener estadísticas de productos
                List<Producto> productos = productoDAL.ObtenerTodosLosProductos();
                lblTotalProductos.Text = productos.Count.ToString();

                int productosAgotados = productos.Count(p => p.CantidadDisponible == 0);
                lblProductosAgotados.Text = productosAgotados.ToString();

                // Obtener estadísticas de cotizaciones
                List<CotizacionCompleta> cotizaciones = cotizacionDAL.ObtenerTodasLasCotizaciones();
                lblTotalCotizaciones.Text = cotizaciones.Count.ToString();

                int cotizacionesVencidas = cotizaciones.Count(c => DateTime.Now.Subtract(c.FechaCotizacion).Days > 30);
                lblCotizacionesVencidas.Text = cotizacionesVencidas.ToString();

                MostrarMensaje("✅ Estadísticas actualizadas correctamente", "alert-success");
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al cargar estadísticas: " + ex.Message, "alert-error");
            }
        }

        #endregion

        #region REQ-REP-01: Reporte de Clientes

        protected void btnGenerarReporteClientes_Click(object sender, EventArgs e)
        {
            try
            {
                List<Cliente> clientes = clienteDAL.ObtenerTodosLosClientes();
                List<Cliente> clientesFiltrados = new List<Cliente>();

                string filtro = ddlFiltroClientes.SelectedValue;
                string tipoCliente = ddlTipoCliente.SelectedValue;

                foreach (Cliente cliente in clientes)
                {
                    bool incluir = false;

                    // Aplicar filtro principal
                    switch (filtro)
                    {
                        case "todos":
                            incluir = true;
                            break;
                        case "activos":
                            incluir = cliente.Activo;
                            break;
                        case "inactivos":
                            incluir = !cliente.Activo;
                            break;
                        case "tipo":
                            incluir = true; // Se filtrará por tipo después
                            break;
                    }

                    // Aplicar filtro de tipo si está seleccionado
                    if (incluir && !string.IsNullOrEmpty(tipoCliente))
                    {
                        incluir = cliente.TipoCliente == tipoCliente;
                    }

                    if (incluir)
                    {
                        clientesFiltrados.Add(cliente);
                    }
                }

                gvReporteClientes.DataSource = clientesFiltrados;
                gvReporteClientes.DataBind();
                pnlReporteClientes.Visible = true;

                if (clientesFiltrados.Count == 0)
                {
                    MostrarMensaje("ℹ️ No se encontraron clientes con los filtros seleccionados", "alert-success");
                }
                else
                {
                    MostrarMensaje($"✅ Reporte generado: {clientesFiltrados.Count} clientes encontrados", "alert-success");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al generar reporte de clientes: " + ex.Message, "alert-error");
            }
        }

        #endregion

        #region REQ-REP-02: Reporte de Productos

        protected void btnGenerarReporteProductos_Click(object sender, EventArgs e)
        {
            try
            {
                List<Producto> productos = productoDAL.ObtenerTodosLosProductos();
                List<Producto> productosFiltrados = new List<Producto>();

                string filtro = ddlFiltroProductos.SelectedValue;
                decimal precioMinimo = 0;
                decimal precioMaximo = decimal.MaxValue;

                // Validar precios
                if (!string.IsNullOrEmpty(txtPrecioMinimo.Text))
                {
                    if (!decimal.TryParse(txtPrecioMinimo.Text, out precioMinimo) || precioMinimo < 0)
                    {
                        MostrarMensaje("⚠️ El precio mínimo debe ser un valor válido mayor o igual a 0", "alert-error");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(txtPrecioMaximo.Text))
                {
                    if (!decimal.TryParse(txtPrecioMaximo.Text, out precioMaximo) || precioMaximo < 0)
                    {
                        MostrarMensaje("⚠️ El precio máximo debe ser un valor válido mayor o igual a 0", "alert-error");
                        return;
                    }
                }

                if (precioMinimo > precioMaximo)
                {
                    MostrarMensaje("⚠️ El precio mínimo no puede ser mayor al precio máximo", "alert-error");
                    return;
                }

                foreach (Producto producto in productos)
                {
                    bool incluir = false;

                    // Aplicar filtro de stock
                    switch (filtro)
                    {
                        case "todos":
                            incluir = true;
                            break;
                        case "disponibles":
                            incluir = producto.CantidadDisponible > 0;
                            break;
                        case "agotados":
                            incluir = producto.CantidadDisponible == 0;
                            break;
                        case "bajo_stock":
                            incluir = producto.CantidadDisponible > 0 && producto.CantidadDisponible <= 5;
                            break;
                    }

                    // Aplicar filtro de precio
                    if (incluir)
                    {
                        incluir = producto.Precio >= precioMinimo && producto.Precio <= precioMaximo;
                    }

                    if (incluir)
                    {
                        productosFiltrados.Add(producto);
                    }
                }

                gvReporteProductos.DataSource = productosFiltrados;
                gvReporteProductos.DataBind();
                pnlReporteProductos.Visible = true;

                if (productosFiltrados.Count == 0)
                {
                    MostrarMensaje("ℹ️ No se encontraron productos con los filtros seleccionados", "alert-success");
                }
                else
                {
                    MostrarMensaje($"✅ Reporte generado: {productosFiltrados.Count} productos encontrados", "alert-success");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al generar reporte de productos: " + ex.Message, "alert-error");
            }
        }

        #endregion

        #region REQ-REP-03: Reporte de Cotizaciones

        protected void btnGenerarReporteCotizaciones_Click(object sender, EventArgs e)
        {
            try
            {
                List<CotizacionCompleta> cotizaciones = cotizacionDAL.ObtenerTodasLasCotizaciones();
                List<CotizacionCompleta> cotizacionesFiltradas = new List<CotizacionCompleta>();

                string filtro = ddlFiltroCotizaciones.SelectedValue;
                DateTime fechaDesde = DateTime.MinValue;
                DateTime fechaHasta = DateTime.MaxValue;

                // Validar fechas
                if (!string.IsNullOrEmpty(txtFechaDesde.Text))
                {
                    if (!DateTime.TryParse(txtFechaDesde.Text, out fechaDesde))
                    {
                        MostrarMensaje("⚠️ La fecha desde no es válida", "alert-error");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(txtFechaHasta.Text))
                {
                    if (!DateTime.TryParse(txtFechaHasta.Text, out fechaHasta))
                    {
                        MostrarMensaje("⚠️ La fecha hasta no es válida", "alert-error");
                        return;
                    }
                    // Ajustar para incluir todo el día
                    fechaHasta = fechaHasta.AddDays(1).AddSeconds(-1);
                }

                if (fechaDesde > fechaHasta)
                {
                    MostrarMensaje("⚠️ La fecha desde no puede ser mayor a la fecha hasta", "alert-error");
                    return;
                }

                foreach (CotizacionCompleta cotizacion in cotizaciones)
                {
                    bool incluir = false;
                    int diasTranscurridos = DateTime.Now.Subtract(cotizacion.FechaCotizacion).Days;

                    // Aplicar filtro de estado
                    switch (filtro)
                    {
                        case "todas":
                            incluir = true;
                            break;
                        case "vigentes":
                            incluir = diasTranscurridos <= 30;
                            break;
                        case "vencidas":
                            incluir = diasTranscurridos > 30;
                            break;
                        case "por_vencer":
                            incluir = diasTranscurridos > 23 && diasTranscurridos <= 30;
                            break;
                    }

                    // Aplicar filtro de fecha
                    if (incluir)
                    {
                        incluir = cotizacion.FechaCotizacion >= fechaDesde && cotizacion.FechaCotizacion <= fechaHasta;
                    }

                    if (incluir)
                    {
                        cotizacionesFiltradas.Add(cotizacion);
                    }
                }

                // Ordenar por fecha descendente
                cotizacionesFiltradas = cotizacionesFiltradas.OrderByDescending(c => c.FechaCotizacion).ToList();

                gvReporteCotizaciones.DataSource = cotizacionesFiltradas;
                gvReporteCotizaciones.DataBind();
                pnlReporteCotizaciones.Visible = true;

                // Calcular estadísticas
                lblTotalCotizacionesReporte.Text = cotizacionesFiltradas.Count.ToString();
                decimal montoTotal = cotizacionesFiltradas.Sum(c => c.Total);
                lblMontoTotalCotizaciones.Text = "₡" + montoTotal.ToString("N2");

                if (cotizacionesFiltradas.Count == 0)
                {
                    MostrarMensaje("ℹ️ No se encontraron cotizaciones con los filtros seleccionados", "alert-success");
                }
                else
                {
                    MostrarMensaje($"✅ Reporte generado: {cotizacionesFiltradas.Count} cotizaciones por un total de ₡{montoTotal:N2}", "alert-success");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al generar reporte de cotizaciones: " + ex.Message, "alert-error");
            }
        }

        #endregion

        #region REQ-REP-04: Reporte de Ventas

        protected void btnGenerarReporteVentas_Click(object sender, EventArgs e)
        {
            try
            {
                List<CotizacionCompleta> cotizaciones = cotizacionDAL.ObtenerTodasLasCotizaciones();
                List<CotizacionCompleta> ventasFiltradas = new List<CotizacionCompleta>();

                string filtro = ddlFiltroVentas.SelectedValue;
                decimal montoMinimo = 0;
                decimal montoMaximo = decimal.MaxValue;

                // Validar montos
                if (!string.IsNullOrEmpty(txtMontoMinimo.Text))
                {
                    if (!decimal.TryParse(txtMontoMinimo.Text, out montoMinimo) || montoMinimo < 0)
                    {
                        MostrarMensaje("⚠️ El monto mínimo debe ser un valor válido mayor o igual a 0", "alert-error");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(txtMontoMaximo.Text))
                {
                    if (!decimal.TryParse(txtMontoMaximo.Text, out montoMaximo) || montoMaximo < 0)
                    {
                        MostrarMensaje("⚠️ El monto máximo debe ser un valor válido mayor o igual a 0", "alert-error");
                        return;
                    }
                }

                if (montoMinimo > montoMaximo)
                {
                    MostrarMensaje("⚠️ El monto mínimo no puede ser mayor al monto máximo", "alert-error");
                    return;
                }

                foreach (CotizacionCompleta cotizacion in cotizaciones)
                {
                    bool incluir = false;

                    // Aplicar filtro de análisis
                    switch (filtro)
                    {
                        case "todas":
                            incluir = true;
                            break;
                        case "monto_alto":
                            incluir = cotizacion.Total > 50000;
                            break;
                        case "monto_medio":
                            incluir = cotizacion.Total >= 10000 && cotizacion.Total <= 50000;
                            break;
                        case "monto_bajo":
                            incluir = cotizacion.Total < 10000;
                            break;
                        case "mes_actual":
                            incluir = cotizacion.FechaCotizacion.Month == DateTime.Now.Month &&
                                     cotizacion.FechaCotizacion.Year == DateTime.Now.Year;
                            break;
                    }

                    // Aplicar filtro de monto personalizado
                    if (incluir)
                    {
                        incluir = cotizacion.Total >= montoMinimo && cotizacion.Total <= montoMaximo;
                    }

                    if (incluir)
                    {
                        ventasFiltradas.Add(cotizacion);
                    }
                }

                // Ordenar por total descendente
                ventasFiltradas = ventasFiltradas.OrderByDescending(v => v.Total).ToList();

                gvReporteVentas.DataSource = ventasFiltradas;
                gvReporteVentas.DataBind();
                pnlReporteVentas.Visible = true;

                // Calcular y mostrar estadísticas de ventas
                CalcularEstadisticasVentas(ventasFiltradas);

                if (ventasFiltradas.Count == 0)
                {
                    MostrarMensaje("ℹ️ No se encontraron ventas con los filtros seleccionados", "alert-success");
                }
                else
                {
                    decimal montoTotal = ventasFiltradas.Sum(v => v.Total);
                    MostrarMensaje($"✅ Reporte generado: {ventasFiltradas.Count} ventas por un total de ₡{montoTotal:N2}", "alert-success");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al generar reporte de ventas: " + ex.Message, "alert-error");
            }
        }

        private void CalcularEstadisticasVentas(List<CotizacionCompleta> ventas)
        {
            if (ventas.Count == 0)
            {
                lblTotalVentas.Text = "0";
                lblMontoTotalVentas.Text = "₡0.00";
                lblPromedioVenta.Text = "₡0.00";
                lblVentaMasAlta.Text = "₡0.00";
                pnlEstadisticasVentas.Visible = false;
                return;
            }

            decimal montoTotal = ventas.Sum(v => v.Total);
            decimal promedio = montoTotal / ventas.Count;
            decimal ventaMasAlta = ventas.Max(v => v.Total);

            lblTotalVentas.Text = ventas.Count.ToString();
            lblMontoTotalVentas.Text = "₡" + montoTotal.ToString("N2");
            lblPromedioVenta.Text = "₡" + promedio.ToString("N2");
            lblVentaMasAlta.Text = "₡" + ventaMasAlta.ToString("N2");

            pnlEstadisticasVentas.Visible = true;
        }

        #endregion

        #region Exportación

        protected void btnExportarTodo_Click(object sender, EventArgs e)
        {
            try
            {
                // Nota: Esta funcionalidad requeriría una librería como iTextSharp para generar PDF
                // Por ahora mostramos un mensaje indicando que la funcionalidad está preparada
                MostrarMensaje("ℹ️ Funcionalidad de exportación a PDF preparada. Requiere implementar librería de PDF (iTextSharp o similar)", "alert-success");

                // Aquí se implementaría la lógica de exportación:
                // 1. Crear documento PDF
                // 2. Agregar estadísticas generales
                // 3. Agregar reportes generados
                // 4. Descargar archivo
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al exportar reporte: " + ex.Message, "alert-error");
            }
        }

        #endregion

        #region Utilidades

        private void MostrarMensaje(string mensaje, string tipo)
        {
            lblMessage.Text = mensaje;
            lblMessage.CssClass = "alert " + tipo;
            pnlMessage.Visible = true;
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        #endregion
    }
}