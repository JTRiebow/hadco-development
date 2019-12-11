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
    ///     A controller for performing operations against Load data in a REST API
    /// </summary>
    public class TruckClassificationsController : GenericController<TruckClassificationDto>
    {
        private readonly ITruckClassificationService _truckClassificationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="truckClassificationService"></param>
        public TruckClassificationsController(ITruckClassificationService truckClassificationService)
            : base(truckClassificationService)
        {
            _truckClassificationService = truckClassificationService;
        }


        /// <summary>
        ///     Returns All Truck Classifications from the system.
        /// </summary>
        /// <response code="404">Not found. Truck Classifications were not found in the system.</response>
        /// <response code="200">OK. The Truck Classifications were found and returned</response>
        [ResponseType(typeof(PaginatedResult<TruckClassificationDto>))]
        [ActionAuthorize("read")]
        [Route("api/TruckClassifications/")]
        public HttpResponseMessage Get()
        {
            return this.Get<TruckClassificationDto>();
        }
    }
}
