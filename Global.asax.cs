using System;
using System.Web;

namespace SistemaCotizaciones
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Código que se ejecuta al iniciar la aplicación
            // Sistema de Cotizaciones iniciado correctamente
        }

        void Application_End(object sender, EventArgs e)
        {
            // Código que se ejecuta al finalizar la aplicación
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Código que se ejecuta cuando se produce un error no controlado
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Código que se ejecuta cuando se inicia una nueva sesión
        }

        void Session_End(object sender, EventArgs e)
        {
            // Código que se ejecuta cuando finaliza una sesión
        }
    }
}