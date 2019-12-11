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
    ///     A controller for performing operations against Equipment data in a REST API
    /// </summary>
    public class EquipmentController : GenericController<EquipmentDto>
    {
        private readonly IEquipmentService _equipmentService;
        
        /// <summary>
        ///     The constructor for the Equipments Controller
        /// </summary>
        /// <param name="equipmentService"></param>
        public EquipmentController(IEquipmentService equipmentService)
            : base(equipmentService)
        {
            _equipmentService = equipmentService;
        }

        /// <summary>
        ///     Gets the Equipments in the system.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<EquipmentDto>))]
        [ActionAuthorize("read")]
        [EndpointQueryStrings]
        [Route("api/Trucks")]
        public HttpResponseMessage GetTrucks()
        {
            int resultCount, totalResultCount;
            var results = _equipmentService.GetTrucks(FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<EquipmentDto>.GetPaginatedResult(Request, results, resultCount, totalResultCount);
        }

        /// <summary>
        ///     Gets the Equipments in the system.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<EquipmentDto>))]
        [ActionAuthorize("read")]
        [EndpointQueryStrings]
        [Route("api/Trailers")]
        public HttpResponseMessage GetTrailers()
        {
            int resultCount, totalResultCount;
            var results = _equipmentService.GetTrailers(FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<EquipmentDto>.GetPaginatedResult(Request, results, resultCount, totalResultCount);
        }

        /// <summary>
        ///     Gets the Equipment in the system.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<EquipmentDto>))]
        [ActionAuthorize("read")]
        [EndpointQueryStrings]
        public HttpResponseMessage Get()
        {
            if ((FilterItems.AllKeys.Contains("filter") && !FilterItems["filter"].Contains("Status")) || !FilterItems.AllKeys.Contains("filter"))
            {
                FilterItems.Add("Status", "1");
            }
            return this.Get<EquipmentDto>();
        }
    }
}
