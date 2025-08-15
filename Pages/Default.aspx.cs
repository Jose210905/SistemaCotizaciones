using System;
using System.Collections.Generic;
using System.Web.UI;
using SistemaCotizaciones.DAL;
using SistemaCotizaciones.Models;

namespace SistemaCotizaciones.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar si el usuario está autenticado
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDatosDashboard();
                VerificarAlertasCotizaciones();
            }
        }

        private void CargarDatosDashboard()
        {
            try
            {
                // Mostrar nombre del usuario
                if (Session["NombreUsuario"] != null)
                {
                    lblUsuario.Text = Session["NombreUsuario"].ToString();
                }

                // Cargar contadores
                CargarContadores();

                // Cargar cotizaciones recientes
                CargarCotizacionesRecientes();
            }
            catch (Exception ex)
            {
                // Manejar errores
                Response.Write("<script>alert('Error al cargar datos: " + ex.Message + "');</script>");
            }
        }

        private void CargarContadores()
        {
            try
            {
                // Contador de clientes
                ClienteDAL clienteDAL = new ClienteDAL();
                var clientes = clienteDAL.ObtenerTodosLosClientes();
                lblTotalClientes.Text = clientes.Count.ToString();

                // Contador de productos
                ProductoDAL productoDAL = new ProductoDAL();
                var productos = productoDAL.ObtenerTodosLosProductos();
                lblTotalProductos.Text = productos.Count.ToString();

                // Contador de cotizaciones activas
                CotizacionDAL cotizacionDAL = new CotizacionDAL();
                var cotizaciones = cotizacionDAL.ObtenerTodasLasCotizaciones();
                lblTotalCotizaciones.Text = cotizaciones.Count.ToString();
            }
            catch (Exception ex)
            {
                lblTotalClientes.Text = "Error";
                lblTotalProductos.Text = "Error";
                lblTotalCotizaciones.Text = "Error";
            }
        }

        private void CargarCotizacionesRecientes()
        {
            try
            {
                CotizacionDAL cotizacionDAL = new CotizacionDAL();
                var cotizaciones = cotizacionDAL.ObtenerTodasLasCotizaciones();

                // Obtener solo las 5 más recientes
                var cotizacionesRecientes = new List<object>();
                int contador = 0;

                foreach (var cotizacion in cotizaciones)
                {
                    if (contador >= 5) break;

                    // Buscar datos del cliente y producto
                    ClienteDAL clienteDAL = new ClienteDAL();
                    ProductoDAL productoDAL = new ProductoDAL();

                    var cliente = clienteDAL.BuscarClientePorIdentificacion(cotizacion.ClienteID.ToString());
                    var producto = productoDAL.BuscarProductoPorCodigo(cotizacion.ProductoID.ToString());

                    cotizacionesRecientes.Add(new
                    {
                        ID = cotizacion.ID,
                        NombreCliente = cliente?.Nombre ?? "Cliente no encontrado",
                        NombreProducto = producto?.Nombre ?? "Producto no encontrado",
                        CantidadProducto = cotizacion.CantidadProducto,
                        Total = cotizacion.Total,
                        FechaCotizacion = cotizacion.FechaCotizacion,
                        Estado = cotizacion.Estado
                    });

                    contador++;
                }

                gvCotizacionesRecientes.DataSource = cotizacionesRecientes;
                gvCotizacionesRecientes.DataBind();
            }
            catch (Exception ex)
            {
                gvCotizacionesRecientes.EmptyDataText = "Error al cargar cotizaciones: " + ex.Message;
                gvCotizacionesRecientes.DataBind();
            }
        }

        private void VerificarAlertasCotizaciones()
        {
            try
            {
                CotizacionDAL cotizacionDAL = new CotizacionDAL();
                var cotizacionesPorVencer = cotizacionDAL.ObtenerCotizacionesPorVencer(7); // 7 días de anticipación

                if (cotizacionesPorVencer.Count > 0)
                {
                    string mensaje = $"<strong>Hay {cotizacionesPorVencer.Count} cotización(es) que vencen pronto:</strong><br/>";

                    foreach (var cotizacion in cotizacionesPorVencer)
                    {
                        // Buscar nombre del cliente
                        ClienteDAL clienteDAL = new ClienteDAL();
                        var cliente = clienteDAL.BuscarClientePorIdentificacion(cotizacion.ClienteID.ToString());

                        string nombreCliente = cliente?.Nombre ?? "Cliente desconocido";
                        string fechaVencimiento = cotizacion.FechaVencimiento?.ToString("dd/MM/yyyy") ?? "Sin fecha";

                        mensaje += $"• Cotización #{cotizacion.ID} - {nombreCliente} - Vence: {fechaVencimiento}<br/>";
                    }

                    lblAlertas.Text = mensaje;
                    pnlAlertas.Visible = true;
                }
                else
                {
                    pnlAlertas.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblAlertas.Text = "Error al verificar alertas: " + ex.Message;
                pnlAlertas.Visible = true;
                pnlAlertas.CssClass = "alert alert-error";
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            // Limpiar sesión
            Session.Clear();
            Session.Abandon();

            // Redirigir al login
            Response.Redirect("Login.aspx");
        }
    }
}