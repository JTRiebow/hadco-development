using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Hadco.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Web.Security;
using Hadco.Common;
using Microsoft.Ajax.Utilities;


namespace Hadco.Web.Controllers
{
    /// <summary>
    ///     A controller for performing operations against EquipmentTimer data in a REST API
    /// </summary>
    public class EquipmentTimersController : GenericController<EquipmentTimerDto>
    {
        private readonly IEquipmentTimerService _equipmentTimerService;
        
        /// <summary>
        ///     The constructor for the EquipmentTimers Controller
        /// </summary>
        /// <param name="equipmentTimerService"></param>
        public EquipmentTimersController(IEquipmentTimerService equipmentTimerService)
            : base(equipmentTimerService)
        {
            _equipmentTimerService = equipmentTimerService;
        }

        /// <summary>
        ///     Creates a new equipment timer in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully created the EquipmentTimer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EquipmentTimerDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Post(EquipmentTimerDto dto)
        {
            return this.Post<EquipmentTimerDto>(dto);
        }

        /// <summary>
        ///    Modifies an existing equipment timer in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully updated the equipment timer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EquipmentTimerDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Patch(int id, Delta<EquipmentTimerDto> dto)
        {
            return this.Patch<EquipmentTimerDto>(id, dto);
        }

        /// <summary>
        ///    Deletes an existing equipment timer in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully deleted the equipment timer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ActionAuthorize("delete")]
        public HttpResponseMessage Delete(int id)
        {
            return this.Delete<EquipmentTimerDto>(id);
        }
    }
}
