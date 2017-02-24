using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HouseService.Startup))]

namespace HouseService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}