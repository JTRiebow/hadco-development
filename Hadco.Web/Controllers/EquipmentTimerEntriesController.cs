using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Description;
using System.Web.Http.OData;
using System.Web.Http.OData.Extensions;
using Hadco.Common;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Hadco.Web.Security;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class EquipmentTimerEntriesController : GenericController<EquipmentTimerEntryDto>
    {
        private IEquipmentTimerEntryService _equipmentTimerEntryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equipmentTimerEntryService"></param>
        public EquipmentTimerEntriesController(IEquipmentTimerEntryService equipmentTimerEntryService)
            : base(equipmentTimerEntryService)
        {
            _equipmentTimerEntryService = equipmentTimerEntryService;
        }


        /// <summary>
        ///     Creates a new equipment timer entry in the system.
        /// </summary>
        /// <returns></returns>
        /// <param name="dto"></param>
        /// <param name="startNow">If true, override start time with current server time</param>
        /// <param name="stopNow">If true, override stop time with current server time</param>
        /// <response code="200">Succesfully created the EquipmentTimerEntry.</response>
        /// <response code="400">Start time is before the stop time</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EquipmentTimerEntryDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Post(EquipmentTimerEntryDto dto, bool startNow = false, bool stopNow = false)
        {
            if (startNow)
            {
                dto.StartTime = DateTimeOffset.Now.RoundToMinute();
            }
            if (stopNow)
            {
                dto.StopTime = DateTimeOffset.Now.RoundToMinute();
            }

            if (dto.StartTime > dto.StopTime)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Start time must be before the stop time.");
            }

            return this.Post<EquipmentTimerEntryDto>(dto);
        }

        /// <summary>
        ///    Modifies an existing equipment timer entry in the system.
        /// </summary>
        /// <returns></returns>
        /// <param name="dto"></param>
        /// <param name="startNow">If true, override start time with current server time</param>
        /// <param name="stopNow">If true, override stop time with current server time</param>
        /// <param name="id"></param>
        /// <response code="200">Successfully updated the equipment timer entry.</response>
        /// <response code="400">Start time is before the stop time</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EquipmentTimerEntryDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Patch(int id, Delta<EquipmentTimerEntryDto> dto, bool startNow = false, bool stopNow = false)
        {
            if (startNow)
            {
                dto.TrySetPropertyValue("StartTime", DateTimeOffset.Now.RoundToMinute());
            }
            if (stopNow)
            {
                dto.TrySetPropertyValue("StopTime", DateTimeOffset.Now.RoundToMinute());
            }

            var equipmentTimerEntryDto = new EquipmentTimerEntryDto();
            dto.Patch(equipmentTimerEntryDto);
            if (equipmentTimerEntryDto.StartTime.HasValue && equipmentTimerEntryDto.StopTime.HasValue && equipmentTimerEntryDto.StartTime > equipmentTimerEntryDto.StopTime)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Start time must be before the stop time.");
            }

            return this.Patch<EquipmentTimerEntryDto>(id, dto, ignoreProperties: "EquipmentTimerID");
        }

        /// <summary>
        ///    Deletes an existing equipment timer entry in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully deleted the equipment timer entry.</response>
        /// <response code="401">Unauthorized access.</response>
        [ActionAuthorize("delete")]
        public HttpResponseMessage Delete(int id)
        {
            return this.Delete<EquipmentTimerEntryDto>(id);
        }

    }

}