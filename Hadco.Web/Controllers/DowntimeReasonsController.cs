using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Web.Models;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class DowntimeReasonsController : DefaultController<DowntimeReasonDto>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="downtimeReasonService"></param>
        public DowntimeReasonsController(IDowntimeReasonService downtimeReasonService) : base(downtimeReasonService)
        {
        }

        /// <summary>
        /// Returns a list of Downtime Reasons. If called from the mobile app, it will only return the Downtime Reasons with the DisplayOnMobile Flag
        /// </summary>
        /// <returns></returns>
        public override HttpResponseMessage Get()
        {
            if (Request.IsFromMobile())
            {
                FilterItems.Add(nameof(DowntimeReasonDto.DisplayOnMobile), true.ToString());
            }
            int resultCount;
            int totalResultCount;
            var result = BaseService.GetAll(FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<DowntimeReasonDto>.GetPaginatedResult(Request, result, resultCount, totalResultCount);
        }
    }
}
