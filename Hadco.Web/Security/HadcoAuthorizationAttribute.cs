using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using Thinktecture.IdentityModel;
using Thinktecture.IdentityModel.WebApi;

namespace Hadco.Web.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class ActionAuthorizeAttribute : ResourceActionAuthorizeAttribute
    {
        private string _action;
        private string[] _resources;

        /// <summary>
        /// 
        /// </summary>
        public ActionAuthorizeAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="resources"></param>
        public ActionAuthorizeAttribute(string action, params string[] resources)
        {
            _action = action;
            _resources = resources;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.IsFromNativeAndroid())
            {
                return false;
            }
            var principal = actionContext.ControllerContext.RequestContext.Principal as ClaimsPrincipal;
            if (principal == null || principal.Identity == null)
            {
                principal = Thinktecture.IdentityModel.Principal.Anonymous;
            }

            if (!string.IsNullOrWhiteSpace(_action))
            {
                if (_resources.Length == 0)
                {
                    _resources = new string[] { actionContext.ControllerContext.ControllerDescriptor.ControllerName };
                }

                return ClaimsAuthorization.CheckAccess(principal, _action, _resources);
            }
            else
            {
                return CheckAccess(actionContext, principal);
            }
        }
    }
}