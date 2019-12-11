using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Common
{
    public static class ClaimsExtensions
    {
        public const string ROLE_KEY = "role";
        public const string EMPLOYEEID_KEY = "employeeid";
        public const string USERNAME_KEY = "username";

        public const string ADMIN_ROLE = "System Admin";
        public const string ANONYMOUS_ROLE = "Anonymous";

        public static string[] GetRoles(this ClaimsPrincipal principal)
        {
            return ((ClaimsPrincipal)principal).Claims.Where(x => x.Type == ROLE_KEY).Select(x => x.Value).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static int? GetEmployeeID(this ClaimsPrincipal principal)
        {
            int employeeid;
            if (int.TryParse(((ClaimsPrincipal)principal).Claims.Where(x => x.Type == EMPLOYEEID_KEY).Select(x => x.Value).FirstOrDefault(), out employeeid))
            {
                return employeeid;
            }
            return null;
        }
    }
}