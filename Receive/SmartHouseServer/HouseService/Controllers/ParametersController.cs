using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using HouseService.DataObjects;
using HouseService.Models;

using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.Mobile.Server.Config;

namespace HouseService.Controllers
{
    public class ParametersController : TableController<Parameters>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Parameters>(context, Request);
        }

        // GET tables/Parameters
        public IQueryable<Parameters> GetAllParameters()
        {
            return Query(); 
        }

        // GET tables/Parameters/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Parameters> GetParameters(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Parameters/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Parameters> PatchParameters(string id, Delta<Parameters> patch)
        {
             return UpdateAsync(id, patch);
        }

        public async void InsertParameters(Parameters item)
        {
            
        }
        // POST tables/Parameters
        public async Task<IHttpActionResult> PostParameters(Parameters item)
        {
            Parameters current = await InsertAsync(item);
            //// Get the settings for the server project.
            //HttpConfiguration config = this.Configuration;
            //MobileAppSettingsDictionary settings =
            //    this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            //// Get the Notification Hubs credentials for the Mobile App.
            //string notificationHubName = settings.NotificationHubName;
            //string notificationHubConnection = settings
            //    .Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

            //// Create the notification hub client.
            //NotificationHubClient hub = NotificationHubClient
            //    .CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            //// Define a WNS payload
            //var windowsToastPayload = @"<toast><visual><binding template=""ToastText01""><text id=""1"">"
            //                        + item.Name + @"</text></binding></visual></toast>";
            //try
            //{
            //    // Send the push notification.
            //    var result = await hub.SendWindowsNativeNotificationAsync(windowsToastPayload);

            //    // Write the success result to the logs.
            //    config.Services.GetTraceWriter().Info(result.State.ToString());
            //}
            //catch (System.Exception ex)
            //{
            //    // Write the failure result to the logs.
            //    config.Services.GetTraceWriter()
            //        .Error(ex.Message, null, "Push.SendAsync Error");
            //}

            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Parameters/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteParameters(string id)
        {
             return DeleteAsync(id);
        }
    }
}
