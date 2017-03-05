using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MieTanakaLocalTheaterCompanyV2.Startup))]
namespace MieTanakaLocalTheaterCompanyV2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
