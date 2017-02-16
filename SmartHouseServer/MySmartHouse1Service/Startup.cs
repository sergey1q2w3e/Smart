using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MySmartHouse1Service.Startup))]

namespace MySmartHouse1Service
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}