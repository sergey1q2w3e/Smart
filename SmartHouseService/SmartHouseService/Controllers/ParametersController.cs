using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using SmartHouseService.DataObjects;
using SmartHouseService.Models;
using SmartHouseService.IoT;

namespace SmartHouseService.Controllers
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
        public async Task<Parameters> PatchParameters(string id, Delta<Parameters> patch)
        {
            Parameters paramUpdated = await UpdateAsync(id, patch);
            await IoTHubManager.SendParameterMessage(paramUpdated);
            return paramUpdated;
        }

        // POST tables/Parameters
        public async Task<IHttpActionResult> PostParameters(Parameters item)
        {
            Parameters current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Parameters/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteParameters(string id)
        {
             return DeleteAsync(id);
        }
    }
}
