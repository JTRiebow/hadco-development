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
    public class AuthActivitiesController : GenericController<AuthActivityDto>
    {
        private readonly IAuthActivityService _authActivityService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authActivityService"></param>
        public AuthActivitiesController(IAuthActivityService authActivityService) : base(authActivityService)
        {
            _authActivityService = authActivityService;
        }

        /// <summary>
        ///     Gets all AuthActivity objects
        /// </summary>
        /// <returns></returns>
        [ActionAuthorize("read")]
        [Route("api/AuthActivities"), HttpGet]
        public HttpResponseMessage GetAllAuthActivities()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _authActivityService.GetAllAuthActivities());
        }
    }
}