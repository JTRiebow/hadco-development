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
using System.Text;
using Hadco.Common.Enums;
using Microsoft.Ajax.Utilities;

namespace Hadco.Web.Controllers
{
    /// <summary>
    ///     A controller for performing operations against and EmployeeTimecard data in a REST API
    /// </summary>
    public class EmployeeTimecardsController : GenericController<EmployeeTimecardDto>
    {
        private readonly IEmployeeTimecardService _employeeTimecardService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeTimecardService"></param>
        public EmployeeTimecardsController(IEmployeeTimecardService employeeTimecardService)
            : base(employeeTimecardService)
        {
            _employeeTimecardService = employeeTimecardService;
        }

        /// <summary>
        ///     Gets the EmployeeTimeCards for a given supervisor and department.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully returned the EmployeeTimecard.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(TimecardWeeklySummaryDto))]
        [ActionAuthorize("read")]
        [Route("api/EmployeeTimecards/Supervisor")]
        [HttpGet]
        public HttpResponseMessage GetSupervisor(DateTime week, int? departmentID = null, bool? accountingApproved = null, bool? supervisorApproved = null, bool? billingApproved = null)
        {

            // bring back data based on userid and role for supervisors.
            var results = _employeeTimecardService.GetTimecardWeeklySummary(week, ProjectConstants.SupervisorRole, null, departmentID, null, accountingApproved, supervisorApproved).ToList();
            return PaginatedResult<TimecardWeeklySummaryDto>.GetPaginatedResult(Request, results, results.Count(), results.Count());
        }

        /// <summary>
        ///     Gets the EmployeeTimeCards for a given week and department.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully returned the EmployeeTimecard.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(TimecardWeeklySummaryDto))]
        [ActionAuthorize("read")]
        [Route("api/EmployeeTimecards/Biller")]
        [HttpGet]
        public HttpResponseMessage GetBilling(DateTime week, int? departmentID = null, bool? accountingApproved = null, bool? supervisorApproved = null, bool? billingApproved = null)
        {

            // bring back data based on userid and role for supervisors.
            var results = _employeeTimecardService.GetTimecardWeeklySummary(week, ProjectConstants.BillingRole, null, departmentID, null, accountingApproved, supervisorApproved).ToList();
            return PaginatedResult<TimecardWeeklySummaryDto>.GetPaginatedResult(Request, results, results.Count(), results.Count());
        }

        /// <summary>
        ///     Gets the EmployeeTimeCards for a given supervisor and department.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully returned the EmployeeTimecard.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(TimecardWeeklySummaryDto))]
        [ActionAuthorize("read")]
        [Route("api/EmployeeTimecards/Accountant")]
        [HttpGet]
        public HttpResponseMessage GetAccountant(DateTime week, int? supervisorID = null, int? departmentID = null, bool? accountingApproved = null, bool? supervisorApproved = null, bool? billingApproved = null)
        {

            // bring back data based on userid and role for supervisors.
            var results = _employeeTimecardService.GetTimecardWeeklySummary(week, ProjectConstants.AccountingRole, supervisorID, departmentID, null, accountingApproved, supervisorApproved).ToList();
            return PaginatedResult<TimecardWeeklySummaryDto>.GetPaginatedResult(Request, results, results.Count, results.Count);
        }

        /// <summary>
        ///     Returns the Timecard Summary for a given week for the currently logged in user
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully returned the EmployeeTimecard.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeTimecardSummaryDto))]
        [ActionAuthorize("read")]
        [Route("api/EmployeeTimecards/Summary/Me")]
        public HttpResponseMessage Get(DateTime week)
        {
            var results = _employeeTimecardService.GetCurrentUserTimecardSummary(week).ToList();
            return PaginatedResult<EmployeeTimecardSummaryDto>.GetPaginatedResult(Request, results, results.Count(), results.Count());
        }

        /// <summary>
        ///     Creates a new EmployeeTimecard entry or returns an existing EmployeeTimecard entry in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully created or returned the EmployeeTimecard.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeTimecardDto))]
        [ActionAuthorize("write")]
        [Route("api/EmployeeTimecards")]
        public HttpResponseMessage Post(EmployeeTimecardPostDto dto)
        {
            var timecard = _employeeTimecardService.GetOrCreateTimecard(dto.EmployeeID, dto.Day, dto.DepartmentID);
            return Request.CreateResponse(HttpStatusCode.OK, timecard);
        }

    }
}
