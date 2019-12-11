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
using Microsoft.Owin.Security;

namespace Hadco.Web.Controllers
{
    /// <summary>
    ///     A controller for performing operations against Job data in a REST API
    /// </summary>
    public class TimesheetsController : GenericController<TimesheetDto>
    {
        private readonly ITimesheetService _timesheetService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timesheetService"></param>
        public TimesheetsController(ITimesheetService timesheetService)
            : base(timesheetService)
        {
            _timesheetService = timesheetService;
        }
        /// <summary>
        ///     Gets Timesheet with the given ID
        /// </summary>>
        /// <returns></returns>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpGet]
        [ActionAuthorize("read")]
        [ResponseType(typeof(TimesheetDto))]
        public HttpResponseMessage Get(int timesheetID)
        {
            return this.Get<TimesheetDto>(timesheetID);
        }

        /// <summary>
        ///     Returns information for the current user logged in.
        /// </summary>
        /// <param name="day">The day the timesheet pertains to.</param>
        /// <param name="departmentID"></param>
        /// <param name="employeeID">The id of the employee</param>
        /// <returns></returns>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("api/Employees/{employeeID}/Timesheets/{day}")]
        [ActionAuthorize("read")]
        [ResponseType(typeof(ExpandedTimesheetDto))]
        public HttpResponseMessage GetEmployeeTimesheet(int employeeID, DateTime day, int departmentID)
        {
            var timesheet = _timesheetService.FindExpanded(employeeID, day, departmentID, Request.IsFromMobile());
            if (timesheet == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception("The timesheet does not exist in the system."));               
            }

            return Request.CreateResponse(HttpStatusCode.OK, timesheet);
        }

        /// <summary>
        /// Gets timesheets for foremann assigned to the superintendent
        /// </summary>
        /// <param name="superintendentID"></param>
        /// <param name="week"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(SuperintendentTimesheetsDto))]
        [Route("api/Timesheets/Superintendent/{superintendentID}")]
        public HttpResponseMessage GetForemanTimesheets(int superintendentID, DateTime week)
        {
            var timesheets = _timesheetService.GetForemanTimesheets(superintendentID, week);

            return Request.CreateResponse(HttpStatusCode.OK, timesheets.OrderBy(x => x.Name));
        }

        /// <summary>
        ///     Creates a new timesheet in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully created the timesheet record.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="400">Bad Request. The record already exists.</response>
        [ResponseType(typeof(TimesheetDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Post(TimesheetDto dto)
        {
            dto.Day = dto.Day.Date;
            if (_timesheetService.Exists(dto.EmployeeID, dto.Day, dto.DepartmentID))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    $"The record already exists in the system for employeeid {dto.EmployeeID} on the day {dto.Day:d} in department {dto.DepartmentID}. Please try GET.");
            }
            return this.Post<TimesheetDto>(dto);
        }

        /// <summary>
        ///     Modifies an existing timesheet in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully updated the timesheet.</response>
        /// <response code="401">Unauthorized access.</response>
        [HttpPatch]
        [ResponseType(typeof(TimesheetDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Patch(int id, Delta<BaseTimesheetDto> dto)
        {
            if (dto == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception("A body is required for PATCH"));

            BaseTimesheetDto item = _timesheetService.FindPatch(id);

            if (item == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception("Item does not exist."));

            dto.Patch(item);
            Validate(item);
            if (!ModelState.IsValid)
                return CreateErrorResponse(ModelState);

            try
            {
                TimesheetDto result = _timesheetService.Update(item);
                return Request.CreateResponse(result);
            }
            catch (ArgumentException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        ///     Returns the Odometer reading for the timesheet for the given employee, department day.
        /// </summary>
        /// <param name="day">The day the timesheet pertains to.</param>
        /// <param name="departmentID"></param>
        /// <param name="employeeID">The id of the employee</param>
        /// <returns></returns>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("api/Employees/{employeeID}/Odometer/{day}/DepartmentID/{departmentID}")]
        [ActionAuthorize("read")]
        [ResponseType(typeof(int))]
        public HttpResponseMessage GetTimesheetOdometer(int employeeID, DateTime day, int departmentID)
        {
            var odometer = _timesheetService.GetOdometer(employeeID, day, departmentID);

            return Request.CreateResponse(HttpStatusCode.OK, odometer);
        }

        /// <summary>
        /// Returns current time from the server (DateTimeOffset.Now)
        /// </summary>
        /// <returns></returns>
        [Route("api/CurrentTime")]
        [AllowAnonymous]
        public HttpResponseMessage GetCurrentTime()
        {
            return Request.CreateResponse(HttpStatusCode.OK, DateTimeOffset.Now);
        }
    }
}
