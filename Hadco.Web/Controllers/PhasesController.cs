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
using Hadco.Web.Security;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class PhasesController : GenericController<PhaseDto>
    {
        private readonly IPhaseService _phaseService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phaseService"></param>
        public PhasesController(IPhaseService phaseService) : base(phaseService)
        {
            _phaseService = phaseService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(PaginatedResult<PhaseDto>))]
        [ActionAuthorize("read")]
        [Route("api/Phases/Metro")]
        public HttpResponseMessage GetMetroPhases()
        {
            var result = _phaseService.GetMetroPhases();
            int resultCount = result.Count();
            return PaginatedResult<PhaseDto>.GetPaginatedResult(Request, result, resultCount, resultCount);
        }
    }
}
