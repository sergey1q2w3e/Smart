using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using SmartHouseService.DataObjects;
using SmartHouseService.Models;
using Owin;

namespace SmartHouseService
{
    public partial class Startup
    {
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
        }
    }

    public class MobileServiceInitializer : CreateDatabaseIfNotExists<MobileServiceContext>
    {
        protected override void Seed(MobileServiceContext context)
        {
            List<Parameters> todoItems = new List<Parameters>
            {
                new Parameters { Id = Guid.NewGuid().ToString(), Name = "Humidity", Value=25 },
                new Parameters { Id = Guid.NewGuid().ToString(), Name = "Temperature", Value=20 },
                new Parameters { Id = Guid.NewGuid().ToString(), Name = "FanMode", Value=0 },
                new Parameters { Id = Guid.NewGuid().ToString(), Name = "FanPower", Value=0 },
                new Parameters { Id = Guid.NewGuid().ToString(), Name = "Door", Value=0 }
            };

            foreach (Parameters todoItem in todoItems)
            {
                context.Set<Parameters>().Add(todoItem);
            }

            base.Seed(context);
        }
    }
}

