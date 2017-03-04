using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using HouseService.DataObjects;
using HouseService.Controllers;
using HouseService.Models;
using Owin;

using Microsoft.ServiceBus.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace HouseService
{
    public partial class Startup
    {
        //static string connectionString = "HostName=myiothubesp8266.azure-devices.net;DeviceId=esp8266;SharedAccessKey=Rz3YKzn22hIMMUu7wL9cBrO02pGbNscLIxFWECUe/Hs=";
        //static string iotHubD2cEndpoint = "messages/events";
        static string deviceID = "esp8266";
        //static EventHubClient eventHubClient;

        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new MobileServiceInitializer());

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    // This middleware is intended to be used locally for debugging. By default, HostName will
                    // only have a value when running in an App Service application.
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }

            app.UseWebApi(config);

            //eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            //var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            ////CancellationTokenSource cts = new CancellationTokenSource();
            //var tasks = new List<Task>();
            //foreach (string partition in d2cPartitions)
            //{
            //    tasks.Add(ReceiveMessagesFromDeviceAsync(partition));
            //}
            //Task.WaitAll(tasks.ToArray());
        }
        private static ParametersController controller;
        

        //private static async Task ReceiveMessagesFromDeviceAsync(string partition)
        //{
        //    var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
        //    while (true)
        //    {
        //        EventData eventData = await eventHubReceiver.ReceiveAsync();
        //        if (eventData == null) continue;

        //        string data = Encoding.UTF8.GetString(eventData.GetBytes());
        //        Parameters p = new Parameters { Name = data, Value = 111 };
        //        //Console.WriteLine("Message received. Partition: {0} Data: '{1}'", partition, data);
        //        await controller.PostParameters(p);
        //    }
        //}
    }

    public class MobileServiceInitializer : CreateDatabaseIfNotExists<MobileServiceContext>
    {
        protected override void Seed(MobileServiceContext context)
        {
            List<Parameters> parameters = new List<Parameters>
            {
                new Parameters { Id = Guid.NewGuid().ToString(), Name = "First item", Value = 1 },
                new Parameters { Id = Guid.NewGuid().ToString(), Name = "Second item", Value = 2 }
            };

            foreach (Parameters param in parameters)
            {
                context.Set<Parameters>().Add(param);
            }

            base.Seed(context);
        }
    }
}

