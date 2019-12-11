using Hadco.Services;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hadco.Web.Security
{
    /// <summary>
    ///
    /// </summary>
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();

            await Task.FromResult(0);   // Added to silence the "This async method lacks 'await' operators and will run
                                        // synchronously" compiler warning.
                                        // http://stackoverflow.com/questions/21155692/suppress-warning-from-empty-async-method#answer-21155826
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                // Enable CORS
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

                var employeeAuthenticationService = NinjectHttpContainer.Resolve<IEmployeeAuthenticationService>();
                bool userAuthenticated = employeeAuthenticationService.Authenticate(context.UserName, context.Password);
                if (!userAuthenticated)
                {
                    context.Rejected();
                    return;
                }

                var employee = employeeAuthenticationService.Find(context.UserName);
                var id = new ClaimsIdentity(context.Options.AuthenticationType, "subject", "role");
                id.AddClaim(new Claim("subject", context.UserName));
                id.AddClaim(new Claim("employeeid", employee.ID.ToString()));

                var roles = employee.Roles.Select(x => x.Name);
                var departments = employee.Departments.Select(x => x.Name);

                foreach (var role in roles)
                {
                    id.AddClaim(new Claim("role", role));
                }
                foreach (var department in departments)
                {
                    id.AddClaim(new Claim("department", department));
                }
                var timeoutMinutes = employee.Departments?.Max(x => x.AuthenticationTimeoutMinutes);
                context.Options.AccessTokenExpireTimeSpan = new TimeSpan(0, timeoutMinutes.Value, 0);
                context.Validated(id);
            }
            catch (Exception e)
            {
                throw e;
            }

            await Task.FromResult(0);   // Added to silence the "This async method lacks 'await' operators and will run
                                        // synchronously" compiler warning.
                                        // http://stackoverflow.com/questions/21155692/suppress-warning-from-empty-async-method#answer-21155826
        }
    }
}