using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Hadco.Services;
using Hadco.Web.Security;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class PermissionsController : AuthorizedController
    {
        private readonly IPermissionsService _permissionsService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionsService"></param>
        public PermissionsController(IPermissionsService permissionsService)
        {
            _permissionsService = permissionsService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("api/Permissions/Me"), HttpGet]
        public HttpResponseMessage GetCurrentUserPermissions()
        {
            var permissions = _permissionsService.GetCurrentUserPermissions();
            return Request.CreateResponse(permissions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("api/Permissions"), HttpGet]
        public HttpResponseMessage GetAllPermissions()
        {
            var permissions = _permissionsService.GetAllPermissions();
            return Request.CreateResponse(permissions);
        }

        /// <summary>
        ///     Gets the activity ids that the current user has.
        /// </summary>
        /// <returns></returns>
        [Route("api/Permissions/Activities/Me")]
        public HttpResponseMessage GetCurrentUserPermissionActivityIds()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _permissionsService.GetCurrentUserPermissionActivityIds());
        }

        /// <summary>
        ///    Gets the activity ids based off of the current user roles and the entered departmentID 
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [Route("api/Permissions/Activities")]
        public HttpResponseMessage GetCurrentUserPermissionActivityIdsByDepartmentId(int departmentId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _permissionsService.GetCurrentUserPermissionActivityIdsByDepartmentId(departmentId));
        }

        /// <summary>
        ///     Gets the csv permissions
        /// </summary>
        /// <returns></returns>
        /// <param name="activityId">List of activity ids</param>
        [Route("api/Permissions/ActivityDepartments"), HttpGet]
        public HttpResponseMessage GetActivityDepartments([FromUri] IEnumerable<int> activityId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _permissionsService.GetActivityDepartments(activityId));
        }

        /// <summary>
        ///     Gets Employee Edit activity ids 
        /// </summary>
        /// <returns></returns>
        /// <param name="employeeID"></param>
        [Route("api/Permissions/EmployeeEditActivities"), HttpGet]
        public HttpResponseMessage GetEmployeeEditPermissionActivityIds(int employeeID)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _permissionsService.GetEmployeeEditPermissionActivityIds(employeeID));
        }
    }
}