using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace VirtoCommerce.ModulesPublishing.Controllers.Api
{
    [RoutePrefix("api/modulespublishing")]
    public class ModulesPublishingController : ApiController
    {
        // GET: api/modulespublishing/
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(new[] { "Hello world!" });
        }
    }
}