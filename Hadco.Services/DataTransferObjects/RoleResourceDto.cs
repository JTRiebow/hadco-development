using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class RoleResourceDto
    {
        public int RoleID { get; set; }
        public RoleDto Role { get; set; }
        public int ResourceID { get; set; }
        public ResourceDto Resource { get; set; }

        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Delete { get; set; }

        public int GetActionValue()
        {
            // Creates a value that can be easily operated against to determine the value
            return ((Read) ? ((int)Action.Read) : 0) +
                   ((Write) ? ((int)Action.Write) : 0) +
                   ((Delete) ? ((int)Action.Delete) : 0) +
                   ((int)Action.Options); // Options always passes for CORS. We can change this if we want granular control over CORS access.
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(RoleResourceDto))
                return false;

            var that = obj as RoleResourceDto;

            return that.Role.Equals(this.Role) && that.Resource.Equals(this.Resource);
        }

        public override int GetHashCode()
        {
            int hash = 37;
            hash = hash * 23 + RoleID.GetHashCode();
            hash = hash * 23 + ResourceID.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return GetRoleResourceKey(Role.Name, Resource.Name);
        }

        public static string GetRoleResourceKey(string role, string resource)
        {
            return (role + "_" + resource).ToUpper();
        }
    }
}
