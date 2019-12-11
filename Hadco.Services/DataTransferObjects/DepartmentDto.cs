using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common.Enums;

namespace Hadco.Services.DataTransferObjects
{
    public class PatchDepartmentDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "DepartmentID")]
        public int ID { get; set; }

        public TrackedByType TrackedBy { get; set; }
    }
    public class DepartmentDto : PatchDepartmentDto
    {

        [MaxLength(128)]
        public string Name { get; set; }

        public int AuthenticationTimeoutMinutes { get; set; }

    }
}
