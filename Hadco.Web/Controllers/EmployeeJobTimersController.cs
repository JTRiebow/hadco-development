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
using Hadco.Common.DataTransferObjects;

namespace Hadco.Web.Controllers
{
    /// <summary>
    ///     A controller for performing operations against Job data in a REST API
    /// </summary>
    public class EmployeeJobTimersController : GenericController<EmployeeJobTimerDto>
    {
        private readonly IEmployeeJobTimerService _employeeJobTimerService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeJobTimerService"></param>
        public EmployeeJobTimersController(IEmployeeJobTimerService employeeJobTimerService)
            : base(employeeJobTimerService)
        {
            _employeeJobTimerService = employeeJobTimerService;
        }


        /// <summary>
        ///     Returns all the EmployeeJobTimers for a given JobTimer.
        /// </summary>
        /// <param name="jobTimerID">The id of the JobTimer</param>
        /// <response code="200">OK.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(PaginatedResult<EmployeeJobTimerExpandedDto>))]
        [ActionAuthorize("read")]
        [Route("api/JobTimers/{jobTimerID}/EmployeeJobTimers")]
        public HttpResponseMessage GetJobTimerEmployeeJobTimers(int jobTimerID)
        {
            FilterItems.Add("JobTimerID", jobTimerID.ToString());
            int resultCount;
            int totalResultCount;
            var result = _employeeJobTimerService.GetAllExpanded(FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<EmployeeJobTimerExpandedDto>.GetPaginatedResult(Request, result, resultCount, totalResultCount);
        }

        /// <summary>
        ///     Returns all the EmployeeJobTimers for a given EmployeeTimer.
        /// </summary>
        /// <param name="employeeTimerID">The id of the EmployeeTimer</param>
        /// <response code="200">OK.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(PaginatedResult<EmployeeJobTimerExpandedDto>))]
        [ActionAuthorize("read")]
        [Route("api/EmployeeTimer/{employeeTimerID}/EmployeeJobTimers")]
        public HttpResponseMessage GetEmployeeTimerEmployeeJobTimers(int employeeTimerID)
        {
            FilterItems.Add("EmployeeTimerID", employeeTimerID.ToString());
            int resultCount;
            int totalResultCount;
            var result = _employeeJobTimerService.GetAllExpanded(FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<EmployeeJobTimerExpandedDto>.GetPaginatedResult(Request, result, resultCount, totalResultCount);
        }


        /// <summary>
        ///     Returns all the EmployeeJobTimers for a given EmployeeTimer.
        /// </summary>
        /// <param name="jobTimerID">The id of the EmployeeTimer</param>
        /// <response code="200">OK.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(PaginatedResult<EmployeeJobTimerSummaryDto>))]
        [ActionAuthorize("read")]
        [Route("api/JobTimers/{jobTimerID}/EmployeeJobTimers/Summary")]
        public HttpResponseMessage GetEmployeeJobTimerSummary(int jobTimerID)
        {
            var result = _employeeJobTimerService.GetEmployeeSummary(jobTimerID);
            var count = result.Count();
            return PaginatedResult<EmployeeJobTimerSummaryDto>.GetPaginatedResult(Request, result, count, count);
        }

        /// <summary>
        ///     Returns all the EmployeeJobTimers for a given Employee, Day and Department.
        /// </summary>
        /// <param name="employeeID">The id of the EmployeeJobTimer</param>
        /// <param name="day">The day of the EmployeeJobTimer record to be returned</param>
        /// <param name="departmentID">The id of the department for the EmployeeJobTimer</param>
        /// <response code="200">OK.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(PaginatedResult<EmployeeJobTimerPrimaryDto>))]
        [ActionAuthorize("read")]
        [Route("api/Employees/{employeeID}/EmployeeJobTimers/{day}")]
        public HttpResponseMessage GetEmployeeJobTimerPrimary(int employeeID, DateTime day, int departmentID)
        {
            int resultCount;
            int totalCount;
            var result = _employeeJobTimerService.GetByEmployeeExpanded(employeeID, day, departmentID, FilterItems, Pagination, out resultCount, out totalCount);
            var count = result.Count();
            return PaginatedResult<EmployeeJobTimerPrimaryDto>.GetPaginatedResult(Request, result, resultCount, totalCount);
        }

        /// <summary>
        ///     Creates a new employee jobt timer in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully created the EmployeeJobTimer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeJobTimerDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Post(EmployeeJobTimerDto dto)
        {
            return this.Post<EmployeeJobTimerDto>(dto);
        }

        /// <summary>
        ///    Modifies an existing employee job timer in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully updated the employee timer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeJobTimerDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Patch(int id, Delta<EmployeeJobTimerDto> dto)
        {
            return this.Patch<EmployeeJobTimerDto>(id, dto);
        }

        /// <summary>
        ///    Deletes an existing employee job timer in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully deleted the employee timer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeJobTimerDto))]
        [ActionAuthorize("delete")]
        public HttpResponseMessage Delete(int id)
        {
            return this.Delete<EmployeeJobTimerDto>(id);
        }
    }
}
