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
using System.Text;
using Microsoft.Ajax.Utilities;

namespace Hadco.Web.Controllers
{
    /// <summary>
    ///     A controller for performing operations against Job data in a REST API
    /// </summary>
    public class JobTimersController : GenericController<JobTimerDto>
    {
        private readonly IJobTimerService _jobTimerService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobTimerService"></param>
        public JobTimersController(IJobTimerService jobTimerService)
            : base(jobTimerService)
        {
            _jobTimerService = jobTimerService;
        }


        /// <summary>
        ///     Returns an expanded JobTimer from the system.
        /// </summary>
        /// <response code="404">Not found. The JobTimer was not found in the system.</response>
        /// <response code="200">OK. The JobTimer was found and returned</response>
        [ResponseType(typeof(ExpandedJobTimerFromJobTimerDto))]
        [ActionAuthorize("read")]
        [Route("api/JobTimers/{jobTimerID}")]
        public HttpResponseMessage Get(int jobTimerID)
        {
            var jobTimer = _jobTimerService.FindExpanded(jobTimerID);
            if(jobTimer == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception("The JobTimer does not exist in the system."));
            }
            return Request.CreateResponse(HttpStatusCode.OK, jobTimer);
        }

        /// <summary>
        ///     Returns all the JobTimers for a given Timesheet.
        /// </summary>
        /// <response code="200">OK.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(PaginatedResult<ExpandedJobTimerFromJobTimerDto>))]
        [ActionAuthorize("read")]
        [Route("api/Timesheet/{timesheetID}/JobTimers")]
        public HttpResponseMessage GetTimesheetJobTimers(int timesheetID)
        {
            int resultCount;
            int totalResultCount;
            var result = _jobTimerService.GetAllExpanded(timesheetID, FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<ExpandedJobTimerDto>.GetPaginatedResult(Request, result, resultCount, totalResultCount);
        }

        /// <summary>
        ///     Returns all the Job Timers associated with a given Employee, Day and Department.
        /// </summary>
        /// <param name="employeeID">The id of the EmployeeJobTimer</param>
        /// <param name="day">The day of the EmployeeJobTimer record to be returned</param>
        /// <param name="departmentID">The id of the department for the EmployeeJobTimer</param>
        /// <response code="200">OK.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(IEnumerable<ExpandedJobTimerFromJobTimerDto>))]
        [ActionAuthorize("read")]
        [Route("api/Employees/{employeeID}/JobTimers/{day}")]
        public HttpResponseMessage GetEmployeeJobTimerPrimary(int employeeID, DateTime day, int departmentID)
        {
            var result = _jobTimerService.GetJobTimersForEmployee(employeeID, day, departmentID);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        ///     Creates a new job timer in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully created the JobTimer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(JobTimerDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Post(JobTimerDto dto)
        {            
            return this.Post<JobTimerDto>(dto);
        }

        /// <summary>
        ///    Modifies an existing job timer in the system.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="jobTimerID">The id of the JobTimer to be patched</param>
        /// <returns></returns>
        /// <response code="200">Successfully updated the job timer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(JobTimerDto))]
        [ActionAuthorize("write")]
        [HttpPatch]
        [Route("api/JobTimers/{jobTimerID}")]
        public HttpResponseMessage Patch(int jobTimerID, Delta<JobTimerDto> dto)
        {
            return this.Patch<JobTimerDto>(jobTimerID, dto);
        }
        /// <summary>
        ///    Deletes the JobTimer.
        /// </summary>
        /// <param name="jobTimerID">The id of the JobTimer to be deleted</param>
        /// <returns></returns>
        /// <response code="200">Successfully deleted the JobTimer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ActionAuthorize("delete")]
        [HttpDelete]
        [Route("api/JobTimers/{jobTimerID}")]
        public HttpResponseMessage Delete(int jobTimerID)
        {
            return this.Delete<JobTimerDto>(jobTimerID);
        }
    }
}
