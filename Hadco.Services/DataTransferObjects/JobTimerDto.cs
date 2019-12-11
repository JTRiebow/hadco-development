using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;

namespace Hadco.Services.DataTransferObjects
{
    public class JobTimerDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "JobTimerID")]
        public int ID { get; set; }

        [Required]
        public int TimesheetID { get; set; }

        [JsonConverter(typeof(TruncateSeconds))]
        public DateTimeOffset? StartTime { get; set; }

        [JsonConverter(typeof(TruncateSeconds))]
        public DateTimeOffset? StopTime { get; set; }

        public double? TotalHours
        {
            get
            {
                var hours = (StopTime - StartTime)?.TotalHours;
                return hours.HasValue ? (double?)Math.Round(hours.Value, 2) : null;
            }
        }

        public int? TotalMinutes => (StopTime - StartTime).WholeTotalMinutes();

        public string Diary { get; set; }

        [MaxLength(50)]
        public string InvoiceNumber { get; set; }

        public decimal? NewQuantity { get; set; }

        [Required]
        public int JobID { get; set; }

        [Required]
        public int PhaseID { get; set; }

        [Required]
        public int CategoryID { get; set; }
    }

    public class ExpandedJobTimerDto : JobTimerDto
    {
        public JobDto Job { get; set; }

        public PhaseDto Phase { get; set; }

        public CategoryDto Category { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public BaseEmployeeDto Supervisor { get; set; }
    }

    public class ExpandedJobTimerFromJobTimerDto : ExpandedJobTimerDto
    {
        public decimal? OtherNewQuantity { get; set; }
        public decimal? PreviousQuantity { get; set; }
        public ICollection<EmployeeJobTimerFromJobTimerDto> EmployeeJobTimers { get; set; }

    }

    public class ExpandedJobTimerFromEmployeeTimerDto : ExpandedJobTimerDto
    {
        public ICollection<EmployeeJobTimerExpandedDto> EmployeeJobTimers { get; set; }

    }

    public class ExpandedJobTimerWithEquipmentDto : ExpandedJobTimerDto
    {
        public ICollection<BaseEquipmentDto> Equipment { get; set; } 
    }
}
