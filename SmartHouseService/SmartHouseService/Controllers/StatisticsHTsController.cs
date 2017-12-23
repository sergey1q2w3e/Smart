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
    public class StatisticsHTsController : TableController<StatisticsHTs>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<StatisticsHTs>(context, Request);
        }

        // GET tables/StatisticsHTs
        [EnableQuery(PageSize = 1000)]
        public IQueryable<StatisticsHTs> GetAllStatisticsHTs()
        {
            return Query(); 
        }

        // GET tables/StatisticsHTs/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<StatisticsHTs> GetStatisticsHTs(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/StatisticsHTs/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<StatisticsHTs> PatchStatisticsHTs(string id, Delta<StatisticsHTs> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/StatisticsHTs
        public async Task<IHttpActionResult> PostStatisticsHTs(StatisticsHTs item)
        {
            StatisticsHTs current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/StatisticsHTs/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteStatisticsHTs(string id)
        {
             return DeleteAsync(id);
        }
    }
}
