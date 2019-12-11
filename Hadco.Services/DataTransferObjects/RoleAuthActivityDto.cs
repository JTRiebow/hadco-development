using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common.Enums;
using Hadco.Data.Entities;
using Newtonsoft.Json;

namespace Hadco.Services.DataTransferObjects
{
    public class RoleAuthActivityDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "RoleAuthActivityID")]
        public int ID { get; set; }

        public int RoleID { get; set; }

        public AuthActivityID AuthActivityID { get; set; }
        
        public bool OwnDepartments { get; set; }
        public bool AllDepartments { get; set; }

        public IEnumerable<int> DepartmentIds { get; set; }
    }

    public class ExpandedRoleAuthActivityDto : RoleAuthActivityDto
    {
        public Role Role { get; set; }
        public AuthActivity AuthActivity { get; set; }
    }
}
