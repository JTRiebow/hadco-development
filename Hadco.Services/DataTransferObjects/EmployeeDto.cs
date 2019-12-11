using Hadco.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Hadco.Services.DataTransferObjects
{
    public class BaseEmployeeDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "EmployeeID")]
        public int ID { get; set; }

        public string Username { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }
    }

    public class PatchPostEmployeeDto : BaseEmployeeDto
    {
        [MaxLength(128)]
        public string Phone { get; set; }

        [MaxLength(256)]
        [MinLength(8)]
        public string Password { get; set; }

        [MaxLength(8)]
        public string Pin { get; set; }

        public int EmployeeTypeID { get; set; }

        public EntityStatus Status { get; set; }

    }

    public class SimpleEmployeeDto : BaseEmployeeDto
    {
        [MaxLength(128)]
        public string EmployeeNumber { get; set; }

        public bool OriginComputerEase { get; set; }
    }

    public class EmployeeDto : SimpleEmployeeDto
    {
        [MaxLength(128)]
        public string Phone { get; set; }

        public EntityStatus Status { get; set; }

        public DateTimeOffset? LastAuthenticatedOn { get; set; }

        public int EmployeeTypeID { get; set; }
        
    }

    public class ExpandedEmployeeDto : EmployeeDto
    {
        public ICollection<DepartmentDto> Departments { get; set; }

        public ICollection<RoleDto> Roles { get; set; }

        public ICollection<EmployeeDto> Supervisors { get; set; }
        public bool? IsClockedIn { get; set; }
    }
}
