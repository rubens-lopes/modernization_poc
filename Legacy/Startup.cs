using Microsoft.Owin;

using ModernizationPoC.Legacy;

using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace ModernizationPoC.Legacy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
