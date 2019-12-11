using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Web.Models;
using Hadco.Web.Security;

namespace Hadco.Web.Controllers
{
    /// <summary>
	/// 
	/// </summary>
    public class RoleAuthActivitiesController : DefaultController<RoleAuthActivityDto>
    {
        private readonly IRoleAuthActivityService _roleAuthActivityService;

        /// <summary>
		/// 
		/// </summary>
		/// <param name="roleAuthActivityService"></param>
		public RoleAuthActivitiesController(IRoleAuthActivityService roleAuthActivityService) : base(roleAuthActivityService)
        {
            _roleAuthActivityService = roleAuthActivityService;
        }

        /// <summary>
        ///     Gets the RoleAuthActivity by id
        /// </summary>
        /// <param name="RoleAuthActivityID"></param>
        /// <returns></returns>
        [ResponseType(typeof(RoleAuthActivityDto))]
        [ActionAuthorize("read")]
        [Route("api/RoleAuthActivities/")]
        public override HttpResponseMessage Get(int RoleAuthActivityID)
        {
            return base.Get(RoleAuthActivityID);
        }

        /// <summary>
        ///     Gets all of the RoleAuthActivities
        /// </summary>
        [ResponseType(typeof(RoleAuthActivityDto))]
        [ActionAuthorize("read")]
        [Route("api/RoleAuthActivities/All/")]
        public HttpResponseMessage GetAllRoleAuthActivities()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _roleAuthActivityService.All);
        }

        /// <summary>
        ///     Inserts RoleAuthActivity
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <response code="404">Not found</response>
        /// <response code="200">Successfully added RoleAuthActivity</response>
        [HttpPost]
        [Route("api/RoleAuthActivities/")]
        [ActionAuthorize("write")]
        public override HttpResponseMessage Post(RoleAuthActivityDto dto)
        {
            return base.Post(dto);
        }

        /// <summary>
        ///     Updates the RoleAuthActivity
        /// </summary>
        /// <param name="roleAuthActivityID"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <response code="200">Successfully updated RoleAuthActivity</response>
        /// <response code="401">Unauthorized access.</response>
        [HttpPatch]
        [ResponseType(typeof(RoleAuthActivityDto))]
        [ActionAuthorize("write")]
        [Route("api/RoleAuthActivities/{roleAuthActivityID}")]
        public override HttpResponseMessage Patch(int roleAuthActivityID, Delta<RoleAuthActivityDto> dto)
        {
            return base.Patch(roleAuthActivityID, dto);
        }

        /// <summary>
        /// Remove the requested RoleAuthActivity, with the given RoleAuthActivityID, from the system.
        /// </summary>
        /// <param name="RoleAuthActivityID">Id value of the RoleAuthActivity to be removed.</param>
        /// <returns></returns>
        /// <response code="200">Successfully Deleted RoleAuthActivity</response>
        [ResponseType(typeof(RoleAuthActivityDto))]
        [ActionAuthorize("delete")]
        [Route("api/RoleAuthActivities/")]
        public override HttpResponseMessage Delete(int RoleAuthActivityID)
        {
            return base.Delete(RoleAuthActivityID);
        }
    }
}