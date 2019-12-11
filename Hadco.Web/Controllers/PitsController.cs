using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Hadco.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Web.Security;
using Hadco.Common;

namespace Hadco.Web.Controllers
{
    /// <summary>
    ///     A controller for performing operations against Pit data in a REST API
    /// </summary>
    public class PitsController : GenericController<PitDto>
    {
        private readonly IPitService _pitService;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pitService"></param>
        public PitsController(IPitService pitService)
            : base(pitService)
        {
            _pitService = pitService;
        }

        /// <summary>
        ///     Gets the Pits in the system. By default it only returns currently active pits.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<PitDto>))]
        [ActionAuthorize("read")]
        [EndpointQueryStrings]
        public HttpResponseMessage Get()
        {
            var result = _pitService.GetFromCache().ToList();
            return PaginatedResult<PitDto>.GetPaginatedResult(Request, result, result.Count, result.Count);
        }
        
    }
}
