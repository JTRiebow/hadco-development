using Hadco.Services.DataTransferObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;

namespace Hadco.Services.DataTransferObjects
{
    public class TimesheetPostDto
    {
        public int EmployeeID { get; set; }

        public int DepartmentID { get; set; }

        [JsonConverter(typeof(DayConverter))]
        public DateTime Day { get; set; }
    }

    public class BaseTimesheetDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "TimesheetID")]
        public int ID { get; set; }

        /// <summary>
        ///     The equipment that was used for the day
        /// </summary>
        public int? EquipmentID { get; set; }

        /// <summary>
        ///     The time in minutes that the equipment was used.
        /// </summary>
        public int EquipmentUseTime { get; set; }

        public int? Odometer { get; set; }

        public string Notes { get; set; }

        public DateTimeOffset? PreTripStart { get; set; }
        public DateTimeOffset? PreTripEnd { get; set; }
        public DateTimeOffset? DepartureTime { get; set; }
        public DateTimeOffset? ArrivalAtJob { get; set; }
        public DateTimeOffset? PostJob { get; set; }
        public DateTimeOffset? PostJobEnd { get; set; }
    }

    public class TimesheetDto : BaseTimesheetDto
    {        
        [Required]
        [JsonConverter(typeof(DayConverter))]
        public DateTime Day { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public int DepartmentID { get; set; }
    }

    public class ExpandedTimesheetDto : TimesheetDto
    {  
        public BaseEquipmentDto Equipment { get; set; }

        public ICollection<ExpandedEmployeeTimerDto> EmployeeTimers { get; set; }

        public ICollection<ExpandedEquipmentTimerEntryDto> EquipmentTimers { get; set; }

        public ICollection<ExpandedJobTimerWithEquipmentDto> JobTimers { get; set; }

        public ICollection<LoadTimerExpandedDto> LoadTimers { get; set; }

        public IEnumerable<DowntimeTimerExpandedDto> DowntimeTimers { get; set; }
    }

}
