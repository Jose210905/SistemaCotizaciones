using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SistemaCotizaciones.Startup))]
namespace SistemaCotizaciones
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
