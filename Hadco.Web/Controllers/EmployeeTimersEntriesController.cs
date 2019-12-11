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
    ///     A controller for performing operations against employee timer entry data in a REST API
    /// </summary>
    public class EmployeeTimerEntriesController : GenericController<EmployeeTimerEntryDto>
    {
        private readonly IEmployeeTimerEntryService _employeeTimerEntryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeTimerEntryService"></param>
        public EmployeeTimerEntriesController(IEmployeeTimerEntryService employeeTimerEntryService)
            : base(employeeTimerEntryService)
        {
            _employeeTimerEntryService = employeeTimerEntryService;
        }

        /// <summary>
        ///     Creates a new employee timer entry in the system.
        /// </summary>
        /// <returns></returns>
        /// <param name="dto"></param>
        /// <param name="startNow">If true, override start time with current server time</param>
        /// <param name="stopNow">If true, override stop time with current server time</param>
        /// <response code="200">Succesfully created the EmployeeTimerEntry.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeTimerEntryDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Post(EmployeeTimerEntryDto dto, bool startNow = false, bool stopNow = false)
        {
            if (startNow)
            {
                dto.ClockIn = DateTimeOffset.Now.RoundToMinute();
            }
            if (stopNow)
            {
                dto.ClockOut = DateTimeOffset.Now.RoundToMinute();
            }

            if (dto.ClockIn > dto.ClockOut)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Clock in time must be before the clock out time.");
            }

            return this.Post<EmployeeTimerEntryDto>(dto);
        }

        /// <summary>
        ///    Modifies an existing employee timer entry in the system.
        /// </summary>
        /// <returns></returns>
        /// <param name="dto"></param>
        /// <param name="startNow">If true, override start time with current server time</param>
        /// <param name="stopNow">If true, override stop time with current server time</param>
        /// <response code="200">Successfully updated the employee timer entry.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeTimerEntryDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Patch(int id, Delta<EmployeeTimerEntryDto> dto, bool startNow = false, bool stopNow = false)
        {
            if (startNow)
            {
                dto.TrySetPropertyValue("ClockIn", DateTimeOffset.Now.RoundToMinute());
            }
            if (stopNow)
            {
                dto.TrySetPropertyValue("ClockOut", DateTimeOffset.Now.RoundToMinute());
            }

            var employeeTimerEntryDto = new EmployeeTimerEntryDto();
            dto.Patch(employeeTimerEntryDto);
            if (employeeTimerEntryDto.ClockOut.HasValue && employeeTimerEntryDto.ClockIn > employeeTimerEntryDto.ClockOut)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Clock in time must be before the clock out time.");
            }

            return this.Patch<EmployeeTimerEntryDto>(id, dto);
        }

        /// <summary>
        ///    Deletes an existing employee timer entry in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully deleted the employee timer entry.</response>
        /// <response code="401">Unauthorized access.</response>
        [ActionAuthorize("delete")]
        public HttpResponseMessage Delete(int id)
        {
            return this.Delete<EmployeeTimerEntryDto>(id);
        }
    }
}
