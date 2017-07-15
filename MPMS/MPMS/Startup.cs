using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MPMS.Startup))]
namespace MPMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
