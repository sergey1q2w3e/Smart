using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using SmartHouseService.DataObjects;
using SmartHouseService.Models;

namespace SmartHouseService.Controllers
{
    public class StatisticsHTController : TableController<StatisticsHT>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<StatisticsHT>(context, Request);
        }

        // GET tables/StatisticsHT
        public IQueryable<StatisticsHT> GetAllStatisticsHT()
        {
            return Query(); 
        }

        // GET tables/StatisticsHT/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<StatisticsHT> GetStatisticsHT(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/StatisticsHT/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<StatisticsHT> PatchStatisticsHT(string id, Delta<StatisticsHT> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/StatisticsHT
        public async Task<IHttpActionResult> PostStatisticsHT(StatisticsHT item)
        {
            StatisticsHT current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/StatisticsHT/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteStatisticsHT(string id)
        {
             return DeleteAsync(id);
        }
    }
}
