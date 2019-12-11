using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class ActivityDepartmentsDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "ActivityID")]
        public int ID { get; set; }

        public IEnumerable<int> DepartmentIds { get; set; }
    }
}
