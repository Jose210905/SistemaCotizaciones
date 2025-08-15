using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaCotizaciones.DAL;
using SistemaCotizaciones.Models;

namespace SistemaCotizaciones.Pages
{
    public partial class Productos : System.Web.UI.Page
    {
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
                MostrarPanelRegistrar();
            }
        }

        #region Navegación entre pestañas

        protected void btnTabRegistrar_Click(object sender, EventArgs e)
        {
            MostrarPanelRegistrar();
        }

        protected void btnTabBuscar_Click(object sender, EventArgs e)
        {
            MostrarPanelBuscar();
        }

        protected void btnTabListar_Click(object sender, EventArgs e)
        {
            MostrarPanelListar();
        }

        private void MostrarPanelRegistrar()
        {
            pnlRegistrar.Visible = true;
            pnlBuscar.Visible = false;
            pnlListar.Visible = false;
            pnlMessage.Visible = false;

            // Actualizar estilos de botones
            btnTabRegistrar.CssClass = "btn btn-primary";
            btnTabBuscar.CssClass = "btn";
            btnTabListar.CssClass = "btn";

            LimpiarCampos();
        }

        private void MostrarPanelBuscar()
        {
            pnlRegistrar.Visible = false;
            pnlBuscar.Visible = true;
            pnlListar.Visible = false;
            pnlMessage.Visible = false;
            pnlProductoEncontrado.Visible = false;

            // Actualizar estilos de botones
            btnTabRegistrar.CssClass = "btn";
            btnTabBuscar.CssClass = "btn btn-primary";
            btnTabListar.CssClass = "btn";

            txtBuscarValor.Text = "";
            ddlTipoBusqueda.SelectedIndex = 0;
        }

        private void MostrarPanelListar()
        {
            pnlRegistrar.Visible = false;
            pnlBuscar.Visible = false;
            pnlListar.Visible = true;
            pnlMessage.Visible = false;

            // Actualizar estilos de botones
            btnTabRegistrar.CssClass = "btn";
            btnTabBuscar.CssClass = "btn";
            btnTabListar.CssClass = "btn btn-primary";

            CargarListaProductos();
        }

        #endregion

        #region Registrar Producto

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                    return;

                // Validar que la cantidad y precio sean válidos
                int cantidad;
                decimal precio;

                if (!int.TryParse(txtCantidadDisponible.Text.Trim(), out cantidad))
                {
                    MostrarMensaje("⚠️ La cantidad debe ser un número válido", "alert-error");
                    return;
                }

                if (!decimal.TryParse(txtPrecio.Text.Trim(), out precio))
                {
                    MostrarMensaje("⚠️ El precio debe ser un número válido", "alert-error");
                    return;
                }

                Producto nuevoProducto = new Producto
                {
                    CodigoCabys = txtCodigoCabys.Text.Trim(),
                    Nombre = txtNombreProducto.Text.Trim(),
                    Descripcion = string.IsNullOrEmpty(txtDescripcion.Text.Trim()) ? null : txtDescripcion.Text.Trim(),
                    CantidadDisponible = cantidad,
                    Precio = precio
                };

                bool registrado = productoDAL.RegistrarProducto(nuevoProducto);

                if (registrado)
                {
                    MostrarMensaje("✅ Producto registrado exitosamente", "alert-success");
                    LimpiarCampos();
                }
                else
                {
                    MostrarMensaje("❌ Producto ya registrado. Verifique el código CABYS.", "alert-error");
                    // Marcar campo de código CABYS con error
                    txtCodigoCabys.CssClass = "form-control error";
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al registrar el producto: " + ex.Message, "alert-error");
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtCodigoCabys.Text = "";
            txtNombreProducto.Text = "";
            txtDescripcion.Text = "";
            txtCantidadDisponible.Text = "";
            txtPrecio.Text = "";

            // Quitar estilos de error
            txtCodigoCabys.CssClass = "form-control";
            txtNombreProducto.CssClass = "form-control";
            txtCantidadDisponible.CssClass = "form-control";
            txtPrecio.CssClass = "form-control";
        }

        #endregion

        #region Buscar y Editar Producto

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string valorBusqueda = txtBuscarValor.Text.Trim();
                string tipoBusqueda = ddlTipoBusqueda.SelectedValue;

                if (string.IsNullOrEmpty(valorBusqueda))
                {
                    MostrarMensaje("⚠️ Ingrese el valor a buscar", "alert-error");
                    return;
                }

                Producto producto = null;

                if (tipoBusqueda == "ID")
                {
                    int id;
                    if (!int.TryParse(valorBusqueda, out id))
                    {
                        MostrarMensaje("⚠️ El ID debe ser un número válido", "alert-error");
                        return;
                    }
                    producto = productoDAL.BuscarProductoPorID(id);
                }
                else if (tipoBusqueda == "CodigoCabys")
                {
                    producto = productoDAL.BuscarProductoPorCodigoCabys(valorBusqueda);
                }

                if (producto != null)
                {
                    // Cargar datos en campos de edición
                    txtEditCodigoCabys.Text = producto.CodigoCabys;
                    txtEditNombreProducto.Text = producto.Nombre;
                    txtEditDescripcion.Text = producto.Descripcion ?? "";
                    txtEditCantidadDisponible.Text = producto.CantidadDisponible.ToString();
                    txtEditPrecio.Text = producto.Precio.ToString("F2");

                    // Verificar alertas de stock
                    if (producto.CantidadDisponible == 0)
                    {
                        lblAlertaStock.Text = "❌ PRODUCTO AGOTADO";
                        lblAlertaStock.CssClass = "alert alert-error";
                        pnlAlertaStock.Visible = true;
                    }
                    else if (producto.CantidadDisponible <= 5)
                    {
                        lblAlertaStock.Text = "⚠️ STOCK BAJO - Solo quedan " + producto.CantidadDisponible + " unidades";
                        lblAlertaStock.CssClass = "alert alert-error";
                        pnlAlertaStock.Visible = true;
                    }
                    else
                    {
                        pnlAlertaStock.Visible = false;
                    }

                    // Guardar ID en ViewState para edición
                    ViewState["ProductoID"] = producto.ID;

                    pnlProductoEncontrado.Visible = true;
                    MostrarMensaje("✅ Producto encontrado", "alert-success");
                }
                else
                {
                    pnlProductoEncontrado.Visible = false;
                    MostrarMensaje("❌ Producto no encontrado", "alert-error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al buscar el producto: " + ex.Message, "alert-error");
            }
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                    return;

                if (ViewState["ProductoID"] == null)
                {
                    MostrarMensaje("❌ Error: No se ha seleccionado un producto", "alert-error");
                    return;
                }

                // Validar que la cantidad y precio sean válidos
                int cantidad;
                decimal precio;

                if (!int.TryParse(txtEditCantidadDisponible.Text.Trim(), out cantidad))
                {
                    MostrarMensaje("⚠️ La cantidad debe ser un número válido", "alert-error");
                    return;
                }

                if (!decimal.TryParse(txtEditPrecio.Text.Trim(), out precio))
                {
                    MostrarMensaje("⚠️ El precio debe ser un número válido", "alert-error");
                    return;
                }

                Producto producto = new Producto
                {
                    ID = (int)ViewState["ProductoID"],
                    CodigoCabys = txtEditCodigoCabys.Text.Trim(),
                    Nombre = txtEditNombreProducto.Text.Trim(),
                    Descripcion = string.IsNullOrEmpty(txtEditDescripcion.Text.Trim()) ? null : txtEditDescripcion.Text.Trim(),
                    CantidadDisponible = cantidad,
                    Precio = precio
                };

                bool editado = productoDAL.EditarProducto(producto);

                if (editado)
                {
                    MostrarMensaje("✅ Producto actualizado exitosamente", "alert-success");

                    // Actualizar alertas de stock después de la edición
                    if (cantidad == 0)
                    {
                        lblAlertaStock.Text = "❌ PRODUCTO AGOTADO";
                        lblAlertaStock.CssClass = "alert alert-error";
                        pnlAlertaStock.Visible = true;
                    }
                    else if (cantidad <= 5)
                    {
                        lblAlertaStock.Text = "⚠️ STOCK BAJO - Solo quedan " + cantidad + " unidades";
                        lblAlertaStock.CssClass = "alert alert-error";
                        pnlAlertaStock.Visible = true;
                    }
                    else
                    {
                        pnlAlertaStock.Visible = false;
                    }
                }
                else
                {
                    MostrarMensaje("❌ Error al actualizar el producto", "alert-error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al editar el producto: " + ex.Message, "alert-error");
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["ProductoID"] == null)
                {
                    MostrarMensaje("❌ Error: No se ha seleccionado un producto", "alert-error");
                    return;
                }

                int productoId = (int)ViewState["ProductoID"];
                bool eliminado = productoDAL.EliminarProducto(productoId);

                if (eliminado)
                {
                    MostrarMensaje("✅ Producto eliminado exitosamente", "alert-success");
                    pnlProductoEncontrado.Visible = false;
                    txtBuscarValor.Text = "";
                    ViewState["ProductoID"] = null;
                }
                else
                {
                    MostrarMensaje("❌ Error al eliminar el producto", "alert-error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al eliminar el producto: " + ex.Message, "alert-error");
            }
        }

        #endregion

        #region Lista de Productos

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarListaProductos();
        }

        private void CargarListaProductos()
        {
            try
            {
                List<Producto> productos = productoDAL.ObtenerTodosLosProductos();
                gvProductos.DataSource = productos;
                gvProductos.DataBind();

                lblTotalProductos.Text = productos.Count.ToString();

                // Contar productos agotados
                int productosAgotados = 0;
                foreach (Producto p in productos)
                {
                    if (p.CantidadDisponible == 0)
                        productosAgotados++;
                }
                lblProductosAgotados.Text = productosAgotados.ToString();

                if (productos.Count == 0)
                {
                    MostrarMensaje("ℹ️ No hay productos registrados en el sistema", "alert-success");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al cargar la lista de productos: " + ex.Message, "alert-error");
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