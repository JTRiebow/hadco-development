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
    public class EmployeesController : GenericController<EmployeeDto>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IRoleService _roleService;

        /// <summary>
        ///     The constructor for the Employees Controller
        /// </summary>
        /// <param name="employeeService"></param>
        /// <param name="departmentService"></param>
        /// <param name="roleService"></param>
        public EmployeesController(IEmployeeService employeeService, IDepartmentService departmentService, IRoleService roleService)
            : base(employeeService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _roleService = roleService;
        }

        /// <summary>
        ///     Gets the Employees in the system.
        /// </summary>
        /// <remarks>By default returns only Active status employees.</remarks>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<ExpandedEmployeeDto>))]
        [ActionAuthorize("read")]
        [EndpointQueryStrings]
        public HttpResponseMessage Get(int? supervisorID = null)
        {
            if ((FilterItems.AllKeys.Contains("filter") && !FilterItems["filter"].Contains("Status")) || !FilterItems.AllKeys.Contains("filter"))
            {
                FilterItems.Add("Status", "1");
            }

            int resultCount;
            int totalResultCount;
            var result = _employeeService.GetAllExpanded(supervisorID, FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<ExpandedEmployeeDto>.GetPaginatedResult(Request, result, resultCount, totalResultCount);
        }

        /// <summary>
        ///     Gets the Supervisors in the system.
        /// </summary>
        /// <remarks>A Supervisor is an employee who has the Supervisor role.</remarks>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<EmployeeDto>))]
        [ActionAuthorize("read")]
        [Route("api/Supervisors")]
        [EndpointQueryStrings]
        public HttpResponseMessage GetSupervisors()
        {
            if ((FilterItems.AllKeys.Contains("filter") && !FilterItems["filter"].Contains("Status")) || !FilterItems.AllKeys.Contains("filter"))
            {
                FilterItems.Add("Status", "1");
            }

            int resultCount;
            int totalResultCount;
            var result = _employeeService.GetAllSupervisors(FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<EmployeeDto>.GetPaginatedResult(Request, result, resultCount, totalResultCount);
        }

        /// <summary>
        ///     Returns the employee 
        /// </summary>
        /// <param name="id">The ID of the employee</param>
        /// <returns></returns>
        [ActionAuthorize("read")]
        [ResponseType(typeof(ExpandedEmployeeDto))]
        public HttpResponseMessage Get(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _employeeService.FindExpanded(id));
        }

        /// <summary>
        ///     Validates an employeeID and pin combination. 
        /// </summary>
        /// <remarks>Returns a 200 if the PIN is valid for the given employee.</remarks>
        /// <param name="employeeID">The ID for the employee whose PIN is being validated.</param>
        /// <param name="pin">The PIN of the employee that is being validated.</param>
        /// <param name="timesheetID">Optional TimesheetID. If sent, pin will be validated by employee on timesheet if they are a supervisor in the department of the timesheet.</param>
        /// <response code="200">OK. The pin was valid.</response>
        /// <response code="404">Not Found. The employeeID and pin combination did not result in a record found.</response>      
        [HttpPost]
        [Route("api/Employees/{employeeID}/Pin/{pin}")]
        [ActionAuthorize("read")]
        public HttpResponseMessage ValidatePin(int employeeID, string pin, int? timesheetID = null)
        {
            bool supervisorApproved;
            if (timesheetID.HasValue && _employeeService.IsValidPin(employeeID, pin, timesheetID.Value, out supervisorApproved))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Valid = true, supervisorApproved });
            }
            if (!timesheetID.HasValue && _employeeService.IsValidPin(employeeID, pin, out supervisorApproved))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Valid = true, supervisorApproved });
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new { Valid = false });
        }


        /// <summary>
        ///     Returns information for the current user logged in.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<ExpandedEmployeeDto>))]
        [HttpGet]
        [Route("api/Employees/Me")]
        [ActionAuthorize("read")]
        public HttpResponseMessage GetCurrentEmployee()
        {
            var currentUserID = Principal.GetEmployeeID();

            if (currentUserID.HasValue)
            {
                var user = _employeeService.FindExpanded(currentUserID.Value);

                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception("The token provided does not match an employee in the system."));
            }

        }

        /// <summary>
        /// Updates only the specified fields of the user
        /// </summary>
        /// <param name="id">The user to be updated</param>
        /// <param name="dto">The fields that will be updated</param>
        /// <returns></returns>
        /// <response code="200">The user that was updated.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Patch(int id, Delta<PatchPostEmployeeDto> dto)
        {
            if (dto == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception("A body is required for PATCH"));

            PatchPostEmployeeDto item = _employeeService.FindPatch(id);

            if (item == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception("Item does not exist."));

            dto.Patch(item);
            Validate(item);
            if (!ModelState.IsValid)
                return CreateErrorResponse(ModelState);

            try
            {
                EmployeeDto result = _employeeService.Update(item);
                return Request.CreateResponse(result);
            }
            catch (ArgumentException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /// <summary>
        ///     Create a new Employee in the Hadco system.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ResponseType(typeof(EmployeeDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Post(PatchPostEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return CreateErrorResponse(ModelState);
            }

            try
            {
                var result = _employeeService.Insert(dto);
                var uri = new Uri(Url.Link(DefaultGetRouteName, new { id = result.ID, controller = ControllerName }));

                var response = Request.CreateResponse(result);
                response.Headers.Location = uri;
                return response;
            }
            catch (ArgumentException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        #region User Roles

        /// <summary>
        ///     Returns Roles for the specified Employee
        /// </summary>
        /// <param name="employeeID">The id of the Employee whose Roles will be returned.</param>
        /// <response code="200">OK. Successfully returns the Departments for the Employee.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Not Found. Returned if the Employee does not exist.</response>
        [ActionAuthorize("read")]
        [Route("api/Employees/{employeeID}/Roles")]
        [HttpGet]
        [EndpointQueryStrings]
        public HttpResponseMessage GetRolesForEmployee(int employeeID)
        {
            int resultCount = 0;
            int totalResultCount = 0;

            if (_employeeService.Exists(employeeID))
            {
                var query = _employeeService.GetRolesForUser(employeeID, base.Pagination, out resultCount, out totalResultCount);
                return PaginatedResult<RoleDto>.GetPaginatedResult(Request, query, resultCount, totalResultCount);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound, "The Employee was not found.");
        }

        /// <summary>
        ///     Adds a Role to an Employee
        /// </summary>
        /// <param name="employeeID">The id of the Employee to add the Role to.</param>
        /// <param name="roleID">The id of the Role to add to the Employee</param>
        /// <response code="201">Created. Successfully added the Role to the Employee.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Not Found. Returned if the Employee does not exist.</response>
        /// <response code="400">Bad Request. Returned if the Employee Role record already exists.</response>
        [ActionAuthorize("write")]
        [HttpPost]
        [Route("api/Employees/{employeeID}/Roles")]
        public HttpResponseMessage AddRole(int employeeID, int roleID)
        {
            if (!_employeeService.Exists(employeeID))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The Employee was not found.");
            }

            if (!_roleService.Exists(roleID))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The Role was not found.");
            }

            if (_employeeService.EmployeeRoleExists(employeeID, roleID))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The Employee Role record already exists.");
            }

            try
            {
                var roleDto = new RoleDto { ID = roleID };
                _employeeService.AddRole(employeeID, roleDto);

                return Request.CreateResponse(HttpStatusCode.Created, "The record was successfully created.");

            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        ///     Removes a Role from an Employee
        /// </summary>
        /// <param name="employeeID">The id of the Employee to remove the Role from.</param>
        /// <param name="roleId">The id of the Role to remove from the Employee.</param>
        /// <returns></returns>
        [ActionAuthorize("delete")]
        [HttpDelete]
        [Route("api/Employees/{employeeID}/Roles/{roleId}")]
        public HttpResponseMessage RemoveRole(int employeeID, int roleId)
        {

            if (!_employeeService.EmployeeRoleExists(employeeID, roleId))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The Employee Role record was not found.");
            }

            try
            {
                _employeeService.RemoveRole(employeeID, roleId);
                return Request.CreateResponse(HttpStatusCode.OK, "The Employee Role record was successfully removed.");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        #endregion User Roles

        #region Departments

        /// <summary>
        ///     Returns Departments for the specified Employee
        /// </summary>
        /// <param name="employeeID">The id of the Employee whose Departments will be returned.</param>
        /// <response code="200">OK. Successfully returns the Departments for the Employee.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Not Found. Returned if the Employee does not exist.</response>
        [ActionAuthorize("read")]
        [ResponseType(typeof(PaginatedResult<DepartmentDto>))]
        [Route("api/Employees/{employeeID}/Departments")]
        [HttpGet]
        [EndpointQueryStrings]
        public HttpResponseMessage GetDepartmentsForEmployee(int employeeID)
        {
            int resultCount = 0;
            int totalResultCount = 0;

            if (_employeeService.Exists(employeeID))
            {
                var query = _employeeService.GetDepartmentsForEmployee(employeeID, base.Pagination, out resultCount, out totalResultCount);
                return PaginatedResult<DepartmentDto>.GetPaginatedResult(Request, query, resultCount, totalResultCount);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound, "The Employee was not found.");
        }

        /// <summary>
        ///     Adds an Department to an Employee
        /// </summary>
        /// <param name="employeeID">The id of the Employee to add the Department to.</param>
        /// <param name="departmentID">The id of the Department to add to the Employee</param>
        /// <response code="201">Created. Successfully added the Department to the Employee.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Not Found. Returned if the Employee does not exist.</response>
        /// <response code="400">Bad Request. Returned if the Employee Department record already exists.</response>
        [ActionAuthorize("write")]
        [HttpPost]
        [Route("api/Employees/{employeeID}/Departments")]
        public HttpResponseMessage AddDepartment(int employeeID, int departmentID)
        {
            if (!_employeeService.Exists(employeeID))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The Employee was not found.");
            }

            if (!_departmentService.Exists(departmentID))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The Department was not found.");
            }

            if (_employeeService.EmployeeDepartmentExists(employeeID, departmentID))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The Employee Department record already exists.");
            }

            try
            {
                var departmentDto = new DepartmentDto { ID = departmentID };
                _employeeService.AddDepartment(employeeID, departmentDto);

                return Request.CreateResponse(HttpStatusCode.Created, "The record was successfully created.");

            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        ///     Removes an Department from an Employee
        /// </summary>
        /// <param name="employeeID">The id of the Employee to remove the Department from.</param>
        /// <param name="departmentID">The id of the Department to remove from the Employee</param>
        /// <response code="200">OK. Successfully removed the Department from the Employee.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Not Found. Returned if the Employee does not have the Department associated with it already.</response>
        [ActionAuthorize("delete")]
        [HttpDelete]
        [Route("api/Employees/{employeeID}/Departments/{departmentID}")]
        public HttpResponseMessage RemoveDepartment(int employeeID, int departmentID)
        {
            if (!_employeeService.EmployeeDepartmentExists(employeeID, departmentID))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The Employee Department record was not found.");
            }

            try
            {
                _employeeService.RemoveDepartment(employeeID, departmentID);
                return Request.CreateResponse(HttpStatusCode.OK, "The Employee Department record was successfully removed.");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        #endregion Departments

        #region Supervisors

        /// <summary>
        ///     Returns Supervisors for the specified Employee
        /// </summary>
        /// <param name="employeeID">The id of the Employee whose Supervisors will be returned.</param>
        /// <response code="200">OK. Successfully returns the Supervisors for the Employee.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Not Found. Returned if the Employee does not exist.</response>
        [ActionAuthorize("read")]
        [ResponseType(typeof(PaginatedResult<EmployeeDto>))]
        [Route("api/Employees/{employeeID}/Supervisors")]
        [HttpGet]
        [EndpointQueryStrings]
        public HttpResponseMessage GetSupervisorsForEmployee(int employeeID)
        {
            int resultCount = 0;
            int totalResultCount = 0;

            if (_employeeService.Exists(employeeID))
            {
                var query = _employeeService.GetSupervisorsForEmployee(employeeID, Pagination, out resultCount, out totalResultCount);
                return PaginatedResult<EmployeeDto>.GetPaginatedResult(Request, query, resultCount, totalResultCount);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound, "The Employee was not found.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="day"></param>
        /// <param name="departmentID"></param>
        /// <param name="employeeID">This dictates the order that the supervisors will be listed - most recently worked under, direct supervisor, other</param>
        [ActionAuthorize("read")]
        [ResponseType(typeof(IEnumerable<EmployeeDailyTimesheetDto>))]
        [Route("api/Supervisors/{day}")]
        [HttpGet]
        public HttpResponseMessage GetDailySupervisorsList(DateTime day, int departmentID, int employeeID)
        {
            var result = _employeeService.GetDailySupervisorsList(day, departmentID, employeeID);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        ///     Adds a Supervisor to an Employee
        /// </summary>
        /// <param name="employeeID">The id of the Employee to add the Supervisor to.</param>
        /// <param name="supervisorID">The id of the Supervisor to add to the Employee</param>
        /// <response code="201">Created. Successfully added the Supervisor to the Employee.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Not Found. Returned if the Employee or Supervisor does not exist.</response>
        /// <response code="400">Bad Request. Returned if the Employee Supervisor record already exists.</response>
        [ActionAuthorize("write")]
        [HttpPost]
        [Route("api/Employees/{employeeID}/Supervisors")]
        public HttpResponseMessage AddSupervisor(int employeeID, int supervisorID)
        {
            if (!_employeeService.Exists(employeeID))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The Employee was not found.");
            }

            if (!_employeeService.Exists(supervisorID))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The Supervisor was not found.");
            }

            if (_employeeService.EmployeeSupervisorExists(employeeID, supervisorID))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The Employee Supervisor record already exists.");
            }

            try
            {
                var supervisorDto = new EmployeeDto { ID = supervisorID };
                _employeeService.AddSupervisor(employeeID, supervisorDto);

                return Request.CreateResponse(HttpStatusCode.Created, "The record was successfully created.");

            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        ///     Removes a Supervisor from an Employee
        /// </summary>
        /// <param name="employeeID">The id of the Employee to remove the Supervisor from.</param>
        /// <param name="supervisorID">The id of the Supervisor to remove from the Employee</param>
        /// <response code="200">OK. Successfully removed the Supervisor from the Employee.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Not Found. Returned if the Employee does not have the Supervisor associated with it already.</response>
        [ActionAuthorize("delete")]
        [HttpDelete]
        [Route("api/Employees/{employeeID}/Supervisors/{supervisorID}")]
        public HttpResponseMessage RemoveSupervisor(int employeeID, int supervisorID)
        {
            if (!_employeeService.EmployeeSupervisorExists(employeeID, supervisorID))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The Employee Supervisor record was not found.");
            }

            try
            {
                _employeeService.RemoveSupervisor(employeeID, supervisorID);
                return Request.CreateResponse(HttpStatusCode.OK, "The Employee Supervisor record was successfully removed.");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        #endregion Supervisors
    }
}
