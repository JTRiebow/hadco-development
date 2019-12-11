using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Common;
using Hadco.Web.Models;
using Hadco.Web.Security;
using Microsoft.Ajax.Utilities;

namespace Hadco.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
    public class DowntimeTimersController : GenericController<DowntimeTimerDto>
    {

        private IDowntimeTimerService _downtimeTimerService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="downtimeTimerService"></param>
        public DowntimeTimersController(IDowntimeTimerService downtimeTimerService) : base(downtimeTimerService)
		{
            _downtimeTimerService = downtimeTimerService;
        }


        /// <summary>
        ///     Updates the DowntimeTimer record
        /// </summary>
        /// <param name="id">The id of the DowntimeTimer to be updated</param>
        /// <param name="dto">The DowntimeTimer that will be updated</param>
        /// <param name="truckerDaily">If true, return resulting Downtime timer in trucker daily format</param>
        /// <param name="startNow">If true, override start time with current server time</param>
        /// <param name="stopNow">If true, override stop time with current server time</param>
        /// <returns></returns>
        /// <response code="200">The user that was updated.</response>
        /// <response code="401">Unauthorized access.</response>
        [ActionAuthorize("write")]
        [HttpPatch]
        public HttpResponseMessage Patch(int id, Delta<DowntimeTimerDto> dto, bool truckerDaily = false, bool startNow = false, bool stopNow = false)
	    {
	        if (startNow)
	        {
	            dto.TrySetPropertyValue("StartTime", DateTimeOffset.Now.RoundToMinute());
	        }
	        if (stopNow)
	        {
	            dto.TrySetPropertyValue("StopTime", DateTimeOffset.Now.RoundToMinute());
	        }
            var result = this.Patch<DowntimeTimerDto>(id, dto);
	        return truckerDaily ? Request.CreateResponse(HttpStatusCode.OK, _downtimeTimerService.GetDowntimeTruckerDaily(id)) : result;
	    }

        /// <summary>
        ///     Creates a new DowntimeTimer record.
        /// </summary>
        /// <param name="dto">The DowntimeTimer that will be updated</param>
        /// <param name="startNow">If true, override start time with current server time</param>
        /// <param name="stopNow">If true, override stop time with current server time</param>
        /// <returns></returns>
        [ResponseType(typeof(DowntimeTimerDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Post(DowntimeTimerDto dto, bool startNow = false, bool stopNow = false)
        {
            if (startNow)
            {
                dto.StartTime = DateTimeOffset.Now.RoundToMinute();
            }
            if (stopNow)
            {
                dto.StopTime = DateTimeOffset.Now.RoundToMinute();
            }
            return this.Post<DowntimeTimerDto>(dto);
        }

        /// <summary>
        ///     Deletes the DowntimeTimer record
        /// </summary>
        /// <param name="id">The id of the DowntimeTimer to be updated</param>
        /// <returns></returns>
        /// <response code="200">The user that was updated.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(DowntimeTimerDto))]
        [ActionAuthorize("delete")]
        public HttpResponseMessage Delete(int id)
        {
            return this.Delete<DowntimeTimerDto>(id);
        }


    }
}
