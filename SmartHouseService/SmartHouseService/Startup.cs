using System.Configuration;
using System.Diagnostics;
using Microsoft.Owin;
using Owin;
using SmartHouseService.IoT;

[assembly: OwinStartup(typeof(SmartHouseService.Startup))]

namespace SmartHouseService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
            //IoTHubManager.StartReceive();
            Debug.WriteLine(ConfigurationManager.ConnectionStrings["IoTHubConnectionString"].ConnectionString);
        }

        //private static int h = 20;
        //private void WithoutHud()
        //{
        //    while (true)
        //    {
        //        Debug.WriteLine(h);
        //        using (var contextService = new MobileServiceContext())
        //        {
        //            Parameters paramChange =
        //                contextService.Parameters.Where(p => p.Name == "Humidity").FirstOrDefault<Parameters>();
        //            if (paramChange != null)
        //            {
        //                paramChange.Value = h++;
        //                contextService.Entry(paramChange).State = EntityState.Modified;

        //            }
        //            else
        //            {
        //                contextService.Parameters.Add(new Parameters()
        //                {
        //                    Name = "Humidity",
        //                    Id = Guid.NewGuid().ToString(),
        //                    Value = h
        //                });
        //            }

        //            try
        //            {
        //                contextService.SaveChanges();
        //            }
        //            catch (DbUpdateConcurrencyException e)
        //            {
        //                Debug.WriteLine(String.Format(e.Message + "\n" + e.Entries.Single().ToString()));
        //            }
        //        }
        //        Thread.Sleep(8000);
        //    }
        //}
    }
}