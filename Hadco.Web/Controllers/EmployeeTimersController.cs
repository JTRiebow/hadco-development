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
    ///     A controller for performing operations against Employee data in a REST API
    /// </summary>
    public class EmployeeTimersController : GenericController<EmployeeTimerDto>
    {
        private readonly IEmployeeTimerService _employeeTimerService;
        private readonly IOccurrenceService _occurrenceService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeTimerService"></param>
        /// <param name="occurrenceService"></param>
        public EmployeeTimersController(IEmployeeTimerService employeeTimerService, IOccurrenceService occurrenceService)
            : base(employeeTimerService)
        {
            _employeeTimerService = employeeTimerService;
            _occurrenceService = occurrenceService;
        }

        /// <summary>
        ///    Deletes an existing employee timer in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully deleted the employee timer.</response>
        /// <response code="401">Unauthorized access.</response>
        [HttpDelete]
        [Route("api/EmployeeTimers/{id}")]
        [ActionAuthorize("delete")]
        public HttpResponseMessage Delete(int id)
        {
            return this.Delete<EmployeeTimerDto>(id);
        }

        /// <summary>
        ///     Returns an EmployeeTimer for a given Employee, Day, and Department.
        /// </summary>
        /// <remarks>This endpoint gets an expanded EmployeeTimer. It is used for getting all the information about an EmployeeTimer in one request.</remarks>
        /// <param name="day">The day the timesheet pertains to.</param>
        /// <param name="employeeID">The id of the employee</param>
        /// <param name="departmentID">The department id of the timer.</param>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("api/Employees/{employeeID}/EmployeeTimer/{day}")]
        [ActionAuthorize("read")]
        [ResponseType(typeof(ExpandedEmployeeTimerDto))]
        public HttpResponseMessage GetEmployeeTimer(int employeeID, DateTime day, int departmentID)
        {
            ExpandedEmployeeTimerDto timer;
            if (Request.IsFromMobile())
            {
                var currentEmployeeID = Principal.GetEmployeeID().Value;
                timer = _employeeTimerService.FindExpanded(employeeID, day, departmentID, currentEmployeeID);
            }
            else
            {
                timer = _employeeTimerService.FindExpanded(employeeID, day, departmentID);
            }
            if (timer == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception("The employee timer does not exist in the system."));
            }

            return Request.CreateResponse(HttpStatusCode.OK, timer);
        }

        /// <summary>
        ///     Returns a 200 if an EmployeeTimer for a given Employee, Day, and Department exists, and a 404 if it doesn't.
        /// </summary>        
        /// <param name="day">The day the timesheet pertains to.</param>
        /// <param name="employeeID">The id of the employee</param>
        /// <param name="departmentID">The department id of the timer.</param>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpHead]
        [Route("api/Employees/{employeeID}/EmployeeTimers/{day}")]
        [ActionAuthorize("read")]
        public HttpResponseMessage EmployeeTimerExists(int employeeID, DateTime day, int departmentID)
        {
            var exists = _employeeTimerService.Exists(employeeID, day, departmentID);
            return Request.CreateResponse(exists ? HttpStatusCode.OK : HttpStatusCode.NotFound);
        }

        /// <summary>
        ///     Returns an list of EmployeeTimers for a given Employee, Day, and Department.
        /// </summary>
        /// <remarks>This endpoint gets an expanded EmployeeTimer. It is used for getting all the information about an EmployeeTimer in one request.</remarks>
        /// <param name="day">The day the timesheet pertains to.</param>
        /// <param name="employeeID">The id of the employee</param>
        /// <param name="departmentID">The department id of the timer.</param>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("api/Employees/{employeeID}/EmployeeTimers/{day}/Department/{departmentID}")]
        [ActionAuthorize("read")]
        [ResponseType(typeof(PaginatedResult<ExpandedEmployeeTimerDto>))]
        public HttpResponseMessage GetEmployeeTimers(int employeeID, DateTime day, int departmentID)
        {

            int resultCount;
            int totalResultCount;
            FilterItems.Add("EmployeeID", employeeID.ToString());
            FilterItems.Add("Day", day.ToString());
            FilterItems.Add("DepartmentID", departmentID.ToString());
            var result = _employeeTimerService.GetAllExpanded(FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<ExpandedEmployeeTimerDto>.GetPaginatedResult(Request, result, resultCount, totalResultCount);
        }


        /// <summary>
        ///     Returns all the EmployeeTimers for a given Timesheet.
        /// </summary>
        /// <response code="200">OK.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(PaginatedResult<ExpandedEmployeeTimerDto>))]
        [ActionAuthorize("read")]
        [Route("api/Timesheet/{timesheetID}/EmployeeTimers")]
        public HttpResponseMessage GetTimesheetJobTimers(int timesheetID)
        {
            int resultCount;
            int totalResultCount;
            FilterItems.Add("TimesheetID", timesheetID.ToString());
            var result = _employeeTimerService.GetAllExpanded(FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<ExpandedEmployeeTimerDto>.GetPaginatedResult(Request, result, resultCount, totalResultCount);
        }

        /// <summary>
        ///     Creates a new EmployeeTimer in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully created the EmployeeTimer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeTimerDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Post(EmployeeTimerDto dto)
        {
            dto.Day = dto.Day.Date;
            // REMOVED CHECK FOR MULTIPLE EMPLOYEE TIMERS. THE FRONT END CAN'T HANDLE MULTIPLE TIMERS YET THOUGH!
            //if (_employeeTimerService.Exists(dto.EmployeeID, dto.Day, dto.DepartmentID))
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
            //        $"The record already exists in the system for employeeid {dto.EmployeeID} on the day {dto.Day:d} in department {dto.DepartmentID}. Please try GET.");
            //}
            return this.Post<EmployeeTimerDto>(dto);
        }

        /// <summary>
        ///    Modifies an existing EmployeeTimer in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully updated the employee timer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeTimerDto))]
        [ActionAuthorize("write")]
        [HttpPatch]
        [Route("api/EmployeeTimers/{id}")]
        public HttpResponseMessage Patch(int id, Delta<EmployeeTimerDto> dto)
        {
            return this.Patch<EmployeeTimerDto>(id, dto);
        }

        /// <summary>
        /// Updates an Employee Timer with the given ID. The dto in this request will update all fields on the employee timer
        /// and update all Employee Job Timers and Employee Job Equipment Timers. It will not affect EmployeeTimerEntries, Occurrences or
        /// andy other child objects. If the labor minutes on the Employee Job Timers is null, it will not be added to the database 
        /// and will be deleted if it was there. If the equipment minutes on the Employee Job Equipment timers is null, it will also not be added. 
        /// If an Employee Job Equipment Timer has non-null equipment minutes, but the parent Employee Job Timer does not have labor minutes, 
        /// the labor minutes of the parent Employee Job Timer will be set to 0.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ForemanEmployeeTimer))]
        [ActionAuthorize("write")]
        [HttpPatch]
        [Route("api/EmployeeTimers/{id}/Foreman")]
        public HttpResponseMessage PatchForemanEmployeeTimer(int id, ForemanEmployeeTimerPatch dto)
        {
            dto.ID = id;
            return Request.CreateResponse(HttpStatusCode.OK, _employeeTimerService.UpdateForemanEmployeeTimer(dto));
        }

        /// <summary>
        /// Gets a list of Employee Timers for the timesheet with the given employeeID, departmentID and day
        /// in a format for easier dispay on the foreman timesheet page. It includes and empty employee job timer
        /// and employee job equipment timer for each valid combination based on existing timers on the timesheet.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ForemanTimesheet))]
        [Route("api/EmployeeTimers/Foreman/{employeeID}/Department/{departmentID}/Day/{day}")]
        public HttpResponseMessage GetForemanEmployeeTimer(int employeeID, DateTime day, int departmentID)
        {
            var timers = _employeeTimerService.GetForemanEmployeeTimers(employeeID, day, departmentID);
            if (timers == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK, timers);
        }

        /// <summary>
        ///     Checks if the current employee is currently clocked in and returns the department ID of the department of the current timer
        /// </summary>
        /// <returns></returns>
        [ActionAuthorize("read")]
        [Route("api/EmployeeTimers/OpenTimerDepartment")]
        public HttpResponseMessage GetOpenTimerDepartmentID()
        {
            var employeeID = Principal.GetEmployeeID();
            if (!employeeID.HasValue)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var departmentID = _employeeTimerService.GetOpenTimerDepartmentID(employeeID.Value);

            return Request.CreateResponse(HttpStatusCode.OK, departmentID);
        }


        /// <summary>
        ///     Approves posted Employee Timers for the accountant.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully approved the Timers.</response>
        /// <response code="401">Unauthorized access.</response>
        [ActionAuthorize("write")]
        [Route("api/EmployeeTimers/Accountant")]
        public HttpResponseMessage PostAccountant(int[] employeeTimerIDs)
        {
            _employeeTimerService.AccountingApproveTimers(employeeTimerIDs);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Approves posted Employee Timers for the supervisor.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully approved the Timers.</response>
        /// <response code="401">Unauthorized access.</response>
        [ActionAuthorize("write")]
        [Route("api/EmployeeTimers/Supervisor")]
        public HttpResponseMessage PostSupervisor(int[] employeeTimerIDs)
        {
            _employeeTimerService.SupervisorApproveTimers(employeeTimerIDs);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///    Sets the flagged field for an Employee Timer entry in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully updated the Employee Timer entry.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeTimerDto))]
        [ActionAuthorize("write")]
        [Route("api/EmployeeTimers/{employeeTimerID}")]
        [HttpPost]
        public HttpResponseMessage UpdateFlagged(int employeeTimerID, bool flag)
        {
            var employeeTimer = _employeeTimerService.Find(employeeTimerID);
            if (employeeTimer == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"The timecard with id {employeeTimerID} was not found in the system.");
            }
            employeeTimer.Flagged = flag;
            return Request.CreateResponse(HttpStatusCode.OK, _employeeTimerService.Update(employeeTimer));
        }

        #region Occurrences

        /// <summary>
        ///     Returns Occurrences for the specified EmployeeTimer
        /// </summary>
        /// <param name="employeeTimerID">The id of the EmployeeTimer that has the occurrences to be returned.</param>
        /// <response code="200">OK. Successfully returns the Occurrences for the EmployeeTimer.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Not Found. Returned if the EmployeeTimer does not exist.</response>
        [ActionAuthorize("read")]
        [ResponseType(typeof(PaginatedResult<OccurrenceDto>))]
        [Route("api/EmployeeTimers/{employeeTimerID}/Occurrences")]
        [HttpGet]
        [EndpointQueryStrings]
        public HttpResponseMessage GetOccurrencesForEmployeeTimer(int employeeTimerID)
        {
            int resultCount = 0;
            int totalResultCount = 0;

            if (_employeeTimerService.Exists(employeeTimerID))
            {
                var query = _employeeTimerService.GetOccurrencesForEmployeeTimer(employeeTimerID, base.Pagination, out resultCount, out totalResultCount);
                return PaginatedResult<OccurrenceDto>.GetPaginatedResult(Request, query, resultCount, totalResultCount);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound, "The EmployeeTimer was not found.");
        }

        /// <summary>
        ///     Adds an Occurrence to an EmployeeTimer
        /// </summary>
        /// <param name="employeeTimerID">The id of the EmployeeTimer to add the Occurrence to.</param>
        /// <param name="occurrenceID">The id of the Occurrence to add to the EmployeeTimer</param>
        /// <response code="201">Created. C:\Users\bwarner\Documents\Visual Studio 2015\Projects\hadco\Hadco.Services\DataTransferObjects\LoadTimerDto.csSuccessfully added the Occurrence to the EmployeeTimer.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Not Found. Returned if the EmployeeTimer does not exist.</response>
        /// <response code="400">Bad Request. Returned if the EmployeeTimer Occurrence record already exists.</response>
        [ActionAuthorize("write")]
        [HttpPost]
        [Route("api/EmployeeTimers/{employeeTimerID}/Occurrences")]
        public HttpResponseMessage AddOccurrence(int employeeTimerID, int occurrenceID)
        {
            if (!_employeeTimerService.Exists(employeeTimerID))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The EmployeeTimer was not found.");
            }

            if (!_occurrenceService.Exists(occurrenceID))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The Occurrence was not found.");
            }

            if (_employeeTimerService.EmployeeTimerOccurrenceExists(employeeTimerID, occurrenceID))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The EmployeeTimer Occurrence record already exists.");
            }

            try
            {
                var occurrenceDto = new OccurrenceDto { ID = occurrenceID };
                _employeeTimerService.AddOccurrence(employeeTimerID, occurrenceDto);

                return Request.CreateResponse(HttpStatusCode.Created, "The record was successfully created.");

            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }


        /// <summary>
        ///     Patches Occurrences for the specified EmployeeTimer
        /// </summary>
        /// <param name="employeeTimerID">The id of the EmployeeTimer that has the occurrences to be returned.</param>
        /// <param name="occurrences"></param>
        /// <response code="200">OK. Successfully returns the Occurrences for the EmployeeTimer.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Not Found. Returned if the EmployeeTimer does not exist.</response>
        [ActionAuthorize("write")]
        [ResponseType(typeof(IEnumerable<OccurrenceDto>))]
        [Route("api/EmployeeTimers/{employeeTimerID}/Occurrences")]
        [HttpPatch]
        public HttpResponseMessage ReplaceOccurrences(int employeeTimerID, IEnumerable<int> occurrences)
        {
            if (!_employeeTimerService.Exists(employeeTimerID))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The EmployeeTimer was not found.");
            }
            ICollection<OccurrenceDto> occurrenceDtos = new List<OccurrenceDto>();
            foreach (var occurrenceID in occurrences)
            {

                if (!_occurrenceService.Exists(occurrenceID))
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        $"The Occurrence {occurrenceID} was not found.");
                }
                occurrenceDtos.Add(_occurrenceService.Find(occurrenceID, tracking: false));
            }

            return Request.CreateResponse(HttpStatusCode.Created, _employeeTimerService.ReplaceOccurrences(employeeTimerID, occurrences));
        }

        /// <summary>
        ///     Removes an Occurrence from an EmployeeTimer
        /// </summary>
        /// <param name="employeeTimerID">The id of the EmployeeTimer to remove the Occurrence from.</param>
        /// <param name="occurrenceID">The id of the Occurrence to remove from the EmployeeTimer</param>
        /// <response code="200">OK. Successfully removed the Occurrence from the EmployeeTimer.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Not Found. Returned if the EmployeeTimer does not have the Occurrence associated with it already.</response>
        [ActionAuthorize("delete")]
        [HttpDelete]
        [Route("api/EmployeeTimers/{employeeTimerID}/Occurrences/{occurrenceID}")]
        public HttpResponseMessage RemoveOccurrence(int employeeTimerID, int occurrenceID)
        {
            if (!_employeeTimerService.EmployeeTimerOccurrenceExists(employeeTimerID, occurrenceID))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The EmployeeTimer Occurrence record was not found.");
            }

            try
            {
                _employeeTimerService.RemoveOccurrence(employeeTimerID, occurrenceID);
                return Request.CreateResponse(HttpStatusCode.OK, "The EmployeeTimer Occurrence record was successfully removed.");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        #endregion Occurrences

    }
}
