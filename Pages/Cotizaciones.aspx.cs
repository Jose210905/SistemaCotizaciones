using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaCotizaciones.DAL;
using SistemaCotizaciones.Models;

namespace SistemaCotizaciones.Pages
{
    public partial class Cotizaciones : System.Web.UI.Page
    {
        private CotizacionDAL cotizacionDAL = new CotizacionDAL();
        private ClienteDAL clienteDAL = new ClienteDAL();
        private ProductoDAL productoDAL = new ProductoDAL();

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
                MostrarPanelCrear();
                CargarProductos();
            }
        }

        #region Navegación entre pestañas

        protected void btnTabCrear_Click(object sender, EventArgs e)
        {
            MostrarPanelCrear();
        }

        protected void btnTabConsultar_Click(object sender, EventArgs e)
        {
            MostrarPanelConsultar();
        }

        protected void btnTabListar_Click(object sender, EventArgs e)
        {
            MostrarPanelListar();
        }

        private void MostrarPanelCrear()
        {
            pnlCrear.Visible = true;
            pnlConsultar.Visible = false;
            pnlListar.Visible = false;
            pnlMessage.Visible = false;

            // Actualizar estilos de botones
            btnTabCrear.CssClass = "btn btn-primary";
            btnTabConsultar.CssClass = "btn";
            btnTabListar.CssClass = "btn";

            LimpiarCamposCrear();
            CargarProductos();
        }

        private void MostrarPanelConsultar()
        {
            pnlCrear.Visible = false;
            pnlConsultar.Visible = true;
            pnlListar.Visible = false;
            pnlMessage.Visible = false;
            pnlCotizacionesCliente.Visible = false;
            pnlDetalleCotizacion.Visible = false;

            // Actualizar estilos de botones
            btnTabCrear.CssClass = "btn";
            btnTabConsultar.CssClass = "btn btn-primary";
            btnTabListar.CssClass = "btn";

            txtConsultarIdentificacion.Text = "";
        }

        private void MostrarPanelListar()
        {
            pnlCrear.Visible = false;
            pnlConsultar.Visible = false;
            pnlListar.Visible = true;
            pnlMessage.Visible = false;

            // Actualizar estilos de botones
            btnTabCrear.CssClass = "btn";
            btnTabConsultar.CssClass = "btn";
            btnTabListar.CssClass = "btn btn-primary";

            CargarListaCotizaciones();
        }

        #endregion

        #region Crear Cotización

        private void CargarProductos()
        {
            try
            {
                List<Producto> productos = productoDAL.ObtenerTodosLosProductos();
                ddlProducto.Items.Clear();
                ddlProducto.Items.Add(new ListItem("Seleccionar producto...", ""));

                foreach (Producto producto in productos)
                {
                    if (producto.CantidadDisponible > 0) // Solo productos con stock
                    {
                        ddlProducto.Items.Add(new ListItem(
                            $"{producto.Nombre} (₡{producto.Precio:N2}) - Stock: {producto.CantidadDisponible}",
                            producto.ID.ToString()
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al cargar productos: " + ex.Message, "alert-error");
            }
        }

        protected void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                string identificacion = txtIdentificacionCliente.Text.Trim();

                if (string.IsNullOrEmpty(identificacion))
                {
                    MostrarMensaje("⚠️ Ingrese la identificación del cliente", "alert-error");
                    return;
                }

                Cliente cliente = clienteDAL.BuscarClientePorIdentificacion(identificacion);

                if (cliente != null)
                {
                    // Mostrar información del cliente
                    lblNombreCliente.Text = cliente.Nombre;
                    lblEmailCliente.Text = cliente.Email ?? "No registrado";
                    lblTelefonoCliente.Text = cliente.Telefono ?? "No registrado";

                    pnlClienteInfo.Visible = true;
                    ViewState["ClienteID"] = cliente.ID;
                    MostrarMensaje("✅ Cliente encontrado", "alert-success");
                }
                else
                {
                    pnlClienteInfo.Visible = false;
                    ViewState["ClienteID"] = null;
                    MostrarMensaje("❌ Cliente no encontrado", "alert-error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al buscar el cliente: " + ex.Message, "alert-error");
            }
        }

        protected void ddlProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlProducto.SelectedValue))
                {
                    pnlProductoInfo.Visible = false;
                    txtPrecioUnitario.Text = "0.00";
                    return;
                }

                int productoId = int.Parse(ddlProducto.SelectedValue);
                Producto producto = productoDAL.BuscarProductoPorID(productoId);

                if (producto != null)
                {
                    // Mostrar información del producto
                    lblNombreProducto.Text = producto.Nombre;
                    lblCodigoCabys.Text = producto.CodigoCabys;
                    lblStockDisponible.Text = producto.CantidadDisponible.ToString();
                    lblPrecioProducto.Text = producto.Precio.ToString("N2");

                    // Actualizar precio unitario
                    txtPrecioUnitario.Text = producto.Precio.ToString("F2");

                    // Verificar alertas de stock
                    if (producto.CantidadDisponible <= 5)
                    {
                        lblAlertaStock.Text = "⚠️ STOCK BAJO - Solo quedan " + producto.CantidadDisponible + " unidades";
                        lblAlertaStock.CssClass = "alert alert-error";
                        pnlAlertaStock.Visible = true;
                    }
                    else
                    {
                        pnlAlertaStock.Visible = false;
                    }

                    pnlProductoInfo.Visible = true;
                    ViewState["ProductoID"] = producto.ID;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al cargar información del producto: " + ex.Message, "alert-error");
            }
        }

        protected void btnCrearCotizacion_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                    return;

                // Validar que el cliente y producto estén seleccionados
                if (ViewState["ClienteID"] == null)
                {
                    MostrarMensaje("❌ Debe buscar y seleccionar un cliente válido", "alert-error");
                    return;
                }

                if (ViewState["ProductoID"] == null)
                {
                    MostrarMensaje("❌ Debe seleccionar un producto", "alert-error");
                    return;
                }

                // Validar cantidad y descuento
                int cantidad;
                decimal descuento;

                if (!int.TryParse(txtCantidad.Text.Trim(), out cantidad) || cantidad <= 0)
                {
                    MostrarMensaje("❌ La cantidad debe ser un número válido mayor a 0", "alert-error");
                    return;
                }

                if (!decimal.TryParse(txtDescuento.Text.Trim(), out descuento) || descuento < 0 || descuento > 100)
                {
                    MostrarMensaje("❌ El descuento debe estar entre 0% y 100%", "alert-error");
                    return;
                }

                // Validar stock disponible
                int productoId = (int)ViewState["ProductoID"];
                Producto producto = productoDAL.BuscarProductoPorID(productoId);

                if (producto.CantidadDisponible < cantidad)
                {
                    MostrarMensaje($"❌ Stock insuficiente. Solo quedan {producto.CantidadDisponible} unidades", "alert-error");
                    return;
                }

                // Crear la cotización
                Cotizacion nuevaCotizacion = new Cotizacion
                {
                    ClienteID = (int)ViewState["ClienteID"],
                    ProductoID = productoId,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio,
                    Descuento = descuento,
                    FechaCotizacion = DateTime.Now
                };

                // Calcular totales
                decimal bruto = nuevaCotizacion.Cantidad * nuevaCotizacion.PrecioUnitario;
                decimal montoDescuento = (bruto * nuevaCotizacion.Descuento) / 100;
                decimal subtotal = bruto - montoDescuento;
                decimal iva = subtotal * 0.13m;
                nuevaCotizacion.Total = subtotal + iva;

                bool creada = cotizacionDAL.CrearCotizacion(nuevaCotizacion);

                if (creada)
                {
                    MostrarMensaje("✅ Cotización creada exitosamente", "alert-success");
                    LimpiarCamposCrear();
                }
                else
                {
                    MostrarMensaje("❌ Error al crear la cotización", "alert-error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al crear la cotización: " + ex.Message, "alert-error");
            }
        }

        protected void btnLimpiarCrear_Click(object sender, EventArgs e)
        {
            LimpiarCamposCrear();
        }

        private void LimpiarCamposCrear()
        {
            txtIdentificacionCliente.Text = "";
            txtCantidad.Text = "";
            txtDescuento.Text = "0";
            txtPrecioUnitario.Text = "0.00";
            txtBruto.Text = "0.00";
            txtIVA.Text = "0.00";
            txtTotal.Text = "0.00";

            ddlProducto.SelectedIndex = 0;

            pnlClienteInfo.Visible = false;
            pnlProductoInfo.Visible = false;
            pnlAlertaStock.Visible = false;

            ViewState["ClienteID"] = null;
            ViewState["ProductoID"] = null;
        }

        #endregion

        #region Consultar Cotización

        protected void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                string identificacion = txtConsultarIdentificacion.Text.Trim();

                if (string.IsNullOrEmpty(identificacion))
                {
                    MostrarMensaje("⚠️ Ingrese la identificación del cliente", "alert-error");
                    return;
                }

                Cliente cliente = clienteDAL.BuscarClientePorIdentificacion(identificacion);

                if (cliente != null)
                {
                    List<CotizacionCompleta> cotizaciones = cotizacionDAL.ObtenerCotizacionesPorCliente(cliente.ID);

                    if (cotizaciones.Count > 0)
                    {
                        gvCotizacionesCliente.DataSource = cotizaciones;
                        gvCotizacionesCliente.DataBind();
                        pnlCotizacionesCliente.Visible = true;
                        MostrarMensaje($"✅ Se encontraron {cotizaciones.Count} cotizaciones para el cliente", "alert-success");
                    }
                    else
                    {
                        pnlCotizacionesCliente.Visible = false;
                        MostrarMensaje("ℹ️ El cliente no tiene cotizaciones registradas", "alert-success");
                    }
                }
                else
                {
                    pnlCotizacionesCliente.Visible = false;
                    MostrarMensaje("❌ Cliente no encontrado", "alert-error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al consultar cotizaciones: " + ex.Message, "alert-error");
            }
        }

        protected void gvCotizacionesCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int cotizacionId = (int)gvCotizacionesCliente.DataKeys[gvCotizacionesCliente.SelectedIndex].Value;
                CotizacionCompleta cotizacion = cotizacionDAL.ObtenerCotizacionCompleta(cotizacionId);

                if (cotizacion != null)
                {
                    // Cargar datos en el panel de detalle
                    txtEditCliente.Text = cotizacion.NombreCliente;
                    txtEditProducto.Text = cotizacion.NombreProducto;
                    txtEditCantidad.Text = cotizacion.Cantidad.ToString();
                    txtEditDescuento.Text = cotizacion.Descuento.ToString("F1");
                    txtEditPrecioUnitario.Text = cotizacion.PrecioUnitario.ToString("F2");

                    // Calcular y mostrar totales
                    RecalcularTotales();

                    ViewState["CotizacionID"] = cotizacionId;
                    ViewState["ProductoOriginalID"] = cotizacion.ProductoID;
                    pnlDetalleCotizacion.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al cargar detalle de cotización: " + ex.Message, "alert-error");
            }
        }

        protected void btnRecalcular_Click(object sender, EventArgs e)
        {
            RecalcularTotales();
        }

        private void RecalcularTotales()
        {
            try
            {
                int cantidad = int.Parse(txtEditCantidad.Text);
                decimal precio = decimal.Parse(txtEditPrecioUnitario.Text);
                decimal descuento = decimal.Parse(txtEditDescuento.Text);

                decimal bruto = cantidad * precio;
                decimal montoDescuento = (bruto * descuento) / 100;
                decimal subtotal = bruto - montoDescuento;
                decimal iva = subtotal * 0.13m;
                decimal total = subtotal + iva;

                txtEditBruto.Text = bruto.ToString("F2");
                txtEditIVA.Text = iva.ToString("F2");
                txtEditTotal.Text = total.ToString("F2");
            }
            catch
            {
                // Si hay error en los cálculos, mostrar 0.00
                txtEditBruto.Text = "0.00";
                txtEditIVA.Text = "0.00";
                txtEditTotal.Text = "0.00";
            }
        }

        protected void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                    return;

                if (ViewState["CotizacionID"] == null)
                {
                    MostrarMensaje("❌ Error: No se ha seleccionado una cotización", "alert-error");
                    return;
                }

                // Validar cantidad y descuento
                int cantidad;
                decimal descuento;

                if (!int.TryParse(txtEditCantidad.Text.Trim(), out cantidad) || cantidad <= 0)
                {
                    MostrarMensaje("❌ La cantidad debe ser un número válido mayor a 0", "alert-error");
                    return;
                }

                if (!decimal.TryParse(txtEditDescuento.Text.Trim(), out descuento) || descuento < 0 || descuento > 100)
                {
                    MostrarMensaje("❌ El descuento debe estar entre 0% y 100%", "alert-error");
                    return;
                }

                // Validar stock disponible
                int productoId = (int)ViewState["ProductoOriginalID"];
                Producto producto = productoDAL.BuscarProductoPorID(productoId);

                if (producto.CantidadDisponible < cantidad)
                {
                    MostrarMensaje($"❌ Stock insuficiente. Solo quedan {producto.CantidadDisponible} unidades", "alert-error");
                    return;
                }

                // Actualizar cotización
                int cotizacionId = (int)ViewState["CotizacionID"];
                decimal total = decimal.Parse(txtEditTotal.Text);

                bool actualizada = cotizacionDAL.EditarCotizacion(cotizacionId, cantidad, descuento, total);

                if (actualizada)
                {
                    MostrarMensaje("✅ Cotización actualizada exitosamente", "alert-success");
                    // Recargar la lista de cotizaciones del cliente
                    btnConsultar_Click(null, null);
                }
                else
                {
                    MostrarMensaje("❌ Error al actualizar la cotización", "alert-error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al guardar cambios: " + ex.Message, "alert-error");
            }
        }

        protected void btnEliminarCotizacion_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["CotizacionID"] == null)
                {
                    MostrarMensaje("❌ Error: No se ha seleccionado una cotización", "alert-error");
                    return;
                }

                int cotizacionId = (int)ViewState["CotizacionID"];
                bool eliminada = cotizacionDAL.EliminarCotizacion(cotizacionId);

                if (eliminada)
                {
                    MostrarMensaje("✅ Cotización eliminada exitosamente", "alert-success");
                    pnlDetalleCotizacion.Visible = false;
                    ViewState["CotizacionID"] = null;
                    ViewState["ProductoOriginalID"] = null;
                    // Recargar la lista de cotizaciones del cliente
                    btnConsultar_Click(null, null);
                }
                else
                {
                    MostrarMensaje("❌ Error al eliminar la cotización", "alert-error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al eliminar la cotización: " + ex.Message, "alert-error");
            }
        }

        #endregion

        #region Lista de Cotizaciones

        protected void btnActualizarLista_Click(object sender, EventArgs e)
        {
            CargarListaCotizaciones();
        }

        private void CargarListaCotizaciones()
        {
            try
            {
                List<CotizacionCompleta> cotizaciones = cotizacionDAL.ObtenerTodasLasCotizaciones();
                gvCotizaciones.DataSource = cotizaciones;
                gvCotizaciones.DataBind();

                lblTotalCotizaciones.Text = cotizaciones.Count.ToString();

                // Contar cotizaciones vencidas (más de 30 días)
                int vencidas = 0;
                foreach (CotizacionCompleta c in cotizaciones)
                {
                    if (DateTime.Now.Subtract(c.FechaCotizacion).Days > 30)
                        vencidas++;
                }
                lblCotizacionesVencidas.Text = vencidas.ToString();

                if (cotizaciones.Count == 0)
                {
                    MostrarMensaje("ℹ️ No hay cotizaciones registradas en el sistema", "alert-success");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al cargar la lista de cotizaciones: " + ex.Message, "alert-error");
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