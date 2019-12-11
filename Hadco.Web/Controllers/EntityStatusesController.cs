using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Hadco.Common;
using System.Net;
using Hadco.Web.Security;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityStatusesController : AuthorizedController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ActionAuthorize("read")]
        public HttpResponseMessage Get()
        {
            Dictionary<int, string> statuses = new Dictionary<int, string>();
            foreach(int value in Enum.GetValues(typeof(EntityStatus)))
            {
                statuses.Add(value, ((EntityStatus)value).ToString());
            }
            return Request.CreateResponse(HttpStatusCode.OK, statuses);
        }
    }
}