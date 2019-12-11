using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Hadco.Services;
using Hadco.Common;
using System.Collections.ObjectModel;
using Hadco.Services.DataTransferObjects;

namespace Hadco.Web.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        /// <summary>
        /// 
        /// </summary>
        public AuthorizationManager()
        {
        }

        private static bool _flush = false;
        /// <summary>
        ///  if the underlying data changes anywhere in the system then this should be called in order to reset user rights.
        /// </summary>
        public static void FlushCachedResources()
        {
            _flush = true;
        }

        private Dictionary<string, int> _roleResources;
        private Dictionary<string, int> RoleResources
        {
            get
            {
                if (_roleResources == null || _flush)
                {
                    IRoleService rs = NinjectHttpContainer.Resolve<IRoleService>();
                    _roleResources = rs.GetAllRoleResourceDto().ToDictionary(x => x.ToString(), y => y.GetActionValue());
                    _flush = false;
                }
                return _roleResources;
            }
        }

        /// <summary>
        ///     The thought in the logic here is that the user must have all action access to every resource for the given request. 
        ///     Typically this will just be one resource and one action ("read users" for instance), but we do support more complex
        ///     requests ("[read, write, delete], [users, roles]" for instance). 
        /// </summary>
        private bool CheckRoleResourceAction(string role, Collection<Claim> resources, Collection<Claim> actions)
        {
            int actionValue = GetActionsValue(actions);
            foreach (var resource in resources)
            {
                var key = RoleResourceDto.GetRoleResourceKey(role, resource.Value);
                if (RoleResources.ContainsKey(key))
                {
                    // bitwise AND operation comparing actions available to those requested.
                    if ((RoleResources[key] & actionValue) != actionValue)
                        return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///  Sum up all the actions provided. Read=1, Write=2, Delete=4.
        /// </summary>
        private int GetActionsValue(Collection<Claim> actions)
        {
            return actions.Select(x => (int)(Hadco.Services.Action)Enum.Parse(typeof(Hadco.Services.Action), x.Value, true))
                   .Distinct()
                   .Sum();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CheckAccess(AuthorizationContext context)
        {
            foreach (var role in context.Principal.GetRoles())
            {
                if (CheckRoleResourceAction(role, context.Resource, context.Action))
                    return true;
            }
            return false;
        }
    }
}