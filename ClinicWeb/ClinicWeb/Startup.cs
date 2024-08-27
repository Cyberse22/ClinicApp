using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClinicWeb.Startup))]
namespace ClinicWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
