using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Owin;
using Owin;
using SmartHouseService.DataObjects;
using SmartHouseService.IoT;
using SmartHouseService.Models;

[assembly: OwinStartup(typeof(SmartHouseService.Startup))]

namespace SmartHouseService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
            //IoTHubManager.StartReceive();
            Thread t = new Thread(WithoutHud);
            t.Start();
        }

        private static int h = 0;
        private void WithoutHud()
        {
            string[] msgs = new[] {"H:23.22;T:33", "D:1", "H:22.11"};

            foreach (string msg in msgs)
            {
                IoTHubManager.ParseAndSave(msg);
                Thread.Sleep(20000);
            }
        }
    }
}