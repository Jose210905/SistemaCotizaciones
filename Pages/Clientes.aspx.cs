using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaCotizaciones.DAL;
using SistemaCotizaciones.Models;

namespace SistemaCotizaciones.Pages
{
    public partial class Clientes : System.Web.UI.Page
    {
        private ClienteDAL clienteDAL = new ClienteDAL();

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
            pnlClienteEncontrado.Visible = false;

            // Actualizar estilos de botones
            btnTabRegistrar.CssClass = "btn";
            btnTabBuscar.CssClass = "btn btn-primary";
            btnTabListar.CssClass = "btn";

            txtBuscarIdentificacion.Text = "";
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

            CargarListaClientes();
        }

        #endregion

        #region Registrar Cliente

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                    return;

                Cliente nuevoCliente = new Cliente
                {
                    Nombre = txtNombre.Text.Trim(),
                    Identificacion = txtIdentificacion.Text.Trim(),
                    Telefono = string.IsNullOrEmpty(txtTelefono.Text.Trim()) ? null : txtTelefono.Text.Trim(),
                    Email = string.IsNullOrEmpty(txtEmail.Text.Trim()) ? null : txtEmail.Text.Trim(),
                    Direccion = string.IsNullOrEmpty(txtDireccion.Text.Trim()) ? null : txtDireccion.Text.Trim(),
                    TipoCliente = string.IsNullOrEmpty(ddlTipoCliente.SelectedValue) ? null : ddlTipoCliente.SelectedValue
                };

                bool registrado = clienteDAL.RegistrarCliente(nuevoCliente);

                if (registrado)
                {
                    MostrarMensaje("✅ Cliente registrado exitosamente", "alert-success");
                    LimpiarCampos();
                }
                else
                {
                    MostrarMensaje("❌ Cliente ya registrado. Verifique la identificación.", "alert-error");
                    // Marcar campo de identificación con error
                    txtIdentificacion.CssClass = "form-control error";
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al registrar el cliente: " + ex.Message, "alert-error");
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtNombre.Text = "";
            txtIdentificacion.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
            txtDireccion.Text = "";
            ddlTipoCliente.SelectedIndex = 0;

            // Quitar estilos de error
            txtNombre.CssClass = "form-control";
            txtIdentificacion.CssClass = "form-control";
        }

        #endregion

        #region Buscar y Editar Cliente

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string identificacion = txtBuscarIdentificacion.Text.Trim();

                if (string.IsNullOrEmpty(identificacion))
                {
                    MostrarMensaje("⚠️ Ingrese la identificación del cliente a buscar", "alert-error");
                    return;
                }

                Cliente cliente = clienteDAL.BuscarClientePorIdentificacion(identificacion);

                if (cliente != null)
                {
                    // Cargar datos en campos de edición
                    txtEditNombre.Text = cliente.Nombre;
                    txtEditIdentificacion.Text = cliente.Identificacion;
                    txtEditTelefono.Text = cliente.Telefono ?? "";
                    txtEditEmail.Text = cliente.Email ?? "";
                    txtEditDireccion.Text = cliente.Direccion ?? "";

                    if (!string.IsNullOrEmpty(cliente.TipoCliente))
                        ddlEditTipoCliente.SelectedValue = cliente.TipoCliente;
                    else
                        ddlEditTipoCliente.SelectedIndex = 0;

                    // Guardar ID en ViewState para edición
                    ViewState["ClienteID"] = cliente.ID;

                    pnlClienteEncontrado.Visible = true;
                    MostrarMensaje("✅ Cliente encontrado", "alert-success");
                }
                else
                {
                    pnlClienteEncontrado.Visible = false;
                    MostrarMensaje("❌ Cliente no encontrado", "alert-error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al buscar el cliente: " + ex.Message, "alert-error");
            }
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                    return;

                if (ViewState["ClienteID"] == null)
                {
                    MostrarMensaje("❌ Error: No se ha seleccionado un cliente", "alert-error");
                    return;
                }

                Cliente cliente = new Cliente
                {
                    ID = (int)ViewState["ClienteID"],
                    Nombre = txtEditNombre.Text.Trim(),
                    Identificacion = txtEditIdentificacion.Text.Trim(),
                    Telefono = string.IsNullOrEmpty(txtEditTelefono.Text.Trim()) ? null : txtEditTelefono.Text.Trim(),
                    Email = string.IsNullOrEmpty(txtEditEmail.Text.Trim()) ? null : txtEditEmail.Text.Trim(),
                    Direccion = string.IsNullOrEmpty(txtEditDireccion.Text.Trim()) ? null : txtEditDireccion.Text.Trim(),
                    TipoCliente = string.IsNullOrEmpty(ddlEditTipoCliente.SelectedValue) ? null : ddlEditTipoCliente.SelectedValue
                };

                bool editado = clienteDAL.EditarCliente(cliente);

                if (editado)
                {
                    MostrarMensaje("✅ Cliente actualizado exitosamente", "alert-success");
                }
                else
                {
                    MostrarMensaje("❌ Error al actualizar el cliente", "alert-error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al editar el cliente: " + ex.Message, "alert-error");
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["ClienteID"] == null)
                {
                    MostrarMensaje("❌ Error: No se ha seleccionado un cliente", "alert-error");
                    return;
                }

                int clienteId = (int)ViewState["ClienteID"];
                bool eliminado = clienteDAL.EliminarCliente(clienteId);

                if (eliminado)
                {
                    MostrarMensaje("✅ Cliente inhabilitado exitosamente", "alert-success");
                    pnlClienteEncontrado.Visible = false;
                    txtBuscarIdentificacion.Text = "";
                    ViewState["ClienteID"] = null;
                }
                else
                {
                    MostrarMensaje("❌ Error al inhabilitar el cliente", "alert-error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al eliminar el cliente: " + ex.Message, "alert-error");
            }
        }

        #endregion

        #region Lista de Clientes

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarListaClientes();
        }

        private void CargarListaClientes()
        {
            try
            {
                List<Cliente> clientes = clienteDAL.ObtenerTodosLosClientes();
                gvClientes.DataSource = clientes;
                gvClientes.DataBind();

                lblTotalClientes.Text = clientes.Count.ToString();

                if (clientes.Count == 0)
                {
                    MostrarMensaje("ℹ️ No hay clientes registrados en el sistema", "alert-success");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al cargar la lista de clientes: " + ex.Message, "alert-error");
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