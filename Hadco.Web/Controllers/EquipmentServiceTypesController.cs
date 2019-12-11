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
    ///     A controller for performing operations against EquipmentServiceType data in a REST API
    /// </summary>
    public class EquipmentServiceTypesController : GenericController<EquipmentServiceTypeDto>
    {
        /// <summary>
        ///     The constructor for the EquipmentServiceTypes Controller
        /// </summary>
        /// <param name="equipmentServiceTypeService"></param>
        public EquipmentServiceTypesController(IEquipmentServiceTypeService equipmentServiceTypeService)
            : base(equipmentServiceTypeService)
        {
        }

        /// <summary>
        ///     Gets the Equipments in the system.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<EquipmentServiceTypeDto>))]
        [ActionAuthorize("read")]
        [EndpointQueryStrings]
        public HttpResponseMessage Get()
        {
            var equip = this.Get<EquipmentServiceTypeDto>();
            return equip;
               
        }
    }
}
