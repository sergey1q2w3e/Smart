using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MySmartHouse1Service.DataObjects;
using MySmartHouse1Service.Models;

namespace MySmartHouse1Service.Controllers
{
    public class Parameters1Controller : TableController<Parameters>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MySmartHouse1Context context = new MySmartHouse1Context();
            DomainManager = new EntityDomainManager<Parameters>(context, Request);
        }

        // GET tables/Parameters1
        public IQueryable<Parameters> GetAllParameters()
        {
            return Query(); 
        }

        // GET tables/Parameters1/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Parameters> GetParameters(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Parameters1/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Parameters> PatchParameters(string id, Delta<Parameters> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Parameters1
        public async Task<IHttpActionResult> PostParameters(Parameters item)
        {
            Parameters current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Parameters1/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteParameters(string id)
        {
             return DeleteAsync(id);
        }
    }
}
