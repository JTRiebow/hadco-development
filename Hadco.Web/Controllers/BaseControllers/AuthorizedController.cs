using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Hadco.Web.Controllers
{
    /// <summary>
    ///     A 
    /// </summary>
    [Authorize]
    public abstract class AuthorizedController : BaseController
    {
		/// <summary>
		/// 
		/// </summary>
        protected ClaimsPrincipal Principal
        {
            get
            {
                return (ClaimsPrincipal)User;
            }
        }
    }
}