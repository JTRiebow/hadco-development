using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class EmployeeJobTimerDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "EmployeeJobTimerID")]
        public int ID { get; set; }

        public int JobTimerID { get; set; }

        public int EmployeeTimerID { get; set; }
        
        public int LaborMinutes { get; set; }
        public int TotalAllocatedMinutes { get; set; }

        public decimal LaborHours => Math.Round(LaborMinutes/(decimal)60, 2);
    }

    public class EmployeeJobTimerExpandedDto : EmployeeJobTimerDto
    {
        public ICollection<EmployeeJobEquipmentTimerExpandedDto> EmployeeJobEquipmentTimers { get; set; }

        public BaseEmployeeDto Supervisor { get; set; }

    }

    public class EmployeeJobTimerFromJobTimerDto : EmployeeJobTimerExpandedDto
    {
        public SimpleEmployeeDto Employee { get; set; }
    }

    public class EmployeeJobTimerPrimaryDto : EmployeeJobTimerFromJobTimerDto
    {
        
        public ExpandedJobTimerDto JobTimer { get; set; }
    }

}
