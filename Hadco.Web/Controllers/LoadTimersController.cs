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
using Hadco.Common.DataTransferObjects;
using Microsoft.Ajax.Utilities;
using System.Web;

namespace Hadco.Web.Controllers
{
    /// <summary>
    ///     A controller for performing operations against Load data in a REST API
    /// </summary>
    public class LoadTimersController : GenericController<LoadTimerDto>
    {
        private readonly ILoadTimerService _loadTimerService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadTimerService"></param>
        public LoadTimersController(ILoadTimerService loadTimerService)
            : base(loadTimerService)
        {
            _loadTimerService = loadTimerService;
        }


        /// <summary>
        ///     Returns an expanded LoadTimer from the system.
        /// </summary>
        /// <response code="404">Not found. The LoadTimer was not found in the system.</response>
        /// <response code="200">OK. The LoadTimer was found and returned</response>
        [ResponseType(typeof(LoadTimerPrimaryDto))]
        [ActionAuthorize("read")]
        [Route("api/LoadTimers/{loadTimerID}")]
        public HttpResponseMessage Get(int loadTimerID)
        {
            var loadTimer = _loadTimerService.FindPrimary(loadTimerID);
            if(loadTimer == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception("The LoadTimer does not exist in the system."));
            }
            return Request.CreateResponse(HttpStatusCode.OK, loadTimer);
        }

        /// <summary>
        ///     Returns all the LoadTimers for a given Timesheet.
        /// </summary>
        /// <response code="200">OK.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(PaginatedResult<LoadTimerExpandedDto>))]
        [ActionAuthorize("read")]
        [Route("api/Timesheet/{timesheetID}/LoadTimers")]
        public HttpResponseMessage GetTimesheetLoadTimers(int timesheetID)
        {
            int resultCount;
            int totalResultCount;
            var result = _loadTimerService.GetAllExpanded(timesheetID, FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<LoadTimerExpandedDto>.GetPaginatedResult(Request, result, resultCount, totalResultCount);
        }

        /// <summary>
        ///     Returns Trucker Dailies for a given date range.
        /// </summary>
        /// <response code="200">OK.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(IEnumerable<TruckerDailyDto>))]
        [ActionAuthorize("read")]
        [Route("api/LoadTimers/{startDate}/{endDate}")]
        public HttpResponseMessage GetLoadTimersByDate(DateTime startDate, DateTime endDate, int? departmentID = null)
        {
            var result = _loadTimerService.GetByDateRange(startDate, endDate, departmentID);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        ///     Creates a new load timer in the system. Load and Dump times can be entered as part of the main object, or as a load timer entry.
        ///     Moving forward, we will only support adding Load and Dump time as a load timer entry
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully created the LoadTimer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(LoadTimerDto))]
        [ActionAuthorize("write")]
        [Route("api/LoadTimers")]
        public HttpResponseMessage Post(LoadTimerDto dto)
        {
            if (Request.IsFromNativeAndroid())
            {
                if (!ModelState.IsValid)
                {
                    CreateErrorResponse(ModelState);
                }
                var result = _loadTimerService.InsertNativeApp(dto);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            return this.Post<LoadTimerDto>(dto);
        }

        /// <summary>
        ///    Modifies an existing load timer in the system.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="loadTimerID">The id of the LoadTimer to be patched</param>
        /// <param name="truckerDaily">If true, returns trucker daily dto</param>
        /// <returns></returns>
        /// <response code="200">Successfully updated the load timer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ActionAuthorize("write")]
        [HttpPatch]
        [Route("api/LoadTimers/{loadTimerID}")]
        public HttpResponseMessage Patch(int loadTimerID, Delta<LoadTimerDto> dto, bool truckerDaily = false)
        {
            var result = this.Patch<LoadTimerDto>(loadTimerID, dto);
            return truckerDaily ? Request.CreateResponse(HttpStatusCode.OK, _loadTimerService.GetLoadTruckerDaily(loadTimerID)) : result;
        }

        /// <summary>
        ///    Deletes the LoadTimer.
        /// </summary>
        /// <param name="loadTimerID">The id of the LoadTimer to be deleted</param>
        /// <returns></returns>
        /// <response code="200">Successfully deleted the LoadTimer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ActionAuthorize("delete")]
        [HttpDelete]
        [Route("api/LoadTimers/{loadTimerID}")]
        public HttpResponseMessage Delete(int loadTimerID)
        {
            return this.Delete<LoadTimerDto>(loadTimerID);
        }
    }
}
