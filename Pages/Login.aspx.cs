using System;
using System.Web;
using System.Web.UI;
using SistemaCotizaciones.DAL;
using SistemaCotizaciones.Models;

namespace SistemaCotizaciones.Pages
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Limpiar cualquier sesión existente
                Session.Clear();

                // Si ya está autenticado, redirigir al dashboard
                if (Session["UsuarioLogueado"] != null)
                {
                    Response.Redirect("Default.aspx");
                }
            }
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que los campos no estén vacíos
                if (string.IsNullOrEmpty(txtUsuario.Text.Trim()) ||
                    string.IsNullOrEmpty(txtContrasena.Text.Trim()))
                {
                    MostrarMensaje("Por favor complete todos los campos", "error");
                    return;
                }

                // Crear instancia de UsuarioDAL
                UsuarioDAL usuarioDAL = new UsuarioDAL();

                // Validar credenciales
                Usuario usuarioValidado = usuarioDAL.ValidarLogin(
                    txtUsuario.Text.Trim(),
                    txtContrasena.Text.Trim()
                );

                if (usuarioValidado != null)
                {
                    // Login exitoso
                    // Guardar información del usuario en la sesión
                    Session["UsuarioLogueado"] = usuarioValidado;
                    Session["NombreUsuario"] = usuarioValidado.NombreUsuario;
                    Session["UsuarioID"] = usuarioValidado.ID;

                    // Limpiar campos
                    LimpiarCampos();

                    // Redirigir al dashboard
                    Response.Redirect("Default.aspx");
                }
                else
                {
                    // Login fallido
                    MostrarMensaje("Usuario o contraseña incorrectos", "error");
                    txtContrasena.Text = ""; // Limpiar solo la contraseña
                    txtContrasena.Focus();
                }
            }
            catch (Exception ex)
            {
                // Error en el proceso de login
                MostrarMensaje("Error al iniciar sesión: " + ex.Message, "error");
            }
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            lblMensaje.Text = mensaje;
            pnlMensaje.Visible = true;

            if (tipo == "error")
            {
                pnlMensaje.CssClass = "alert alert-error";
            }
            else if (tipo == "success")
            {
                pnlMensaje.CssClass = "alert alert-success";
            }
        }

        private void LimpiarCampos()
        {
            txtUsuario.Text = "";
            txtContrasena.Text = "";
            pnlMensaje.Visible = false;
        }
    }
}