using Hadco.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Hadco.Services.DataTransferObjects
{
    public class EmployeeTimerDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "EmployeeTimerID")]
        public int ID { get; set; }

        public int? TimesheetID { get; set; }

        public bool Injured { get; set; }

        [Required]
        [JsonConverter(typeof(DayConverter))]
        public DateTime Day { get; set; }

        public int? EquipmentID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        public double TotalHours { get; set; }
        public int TotalMinutes { get; set; }

        public int? ShopMinutes { get; set; }
        public int? TravelMinutes { get; set; }
        public int? GreaseMinutes { get; set; }
        public int? DailyMinutes { get; set; }

        /// <summary>
        ///     The time in minutes that the equipment was used.
        /// </summary>
        public int EquipmentUseTime { get; set; }

        public bool Submitted { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        public int SubDepartmentID { get; set; }

        public int? EmployeeTimecardID { get; set; }

        public bool ApprovedBySupervisor { get; set; }

        public bool ApprovedByAccounting { get; set; }

        public int? ApprovedBySupervisorEmployeeID { get; set; }

        public DateTimeOffset? ApprovedBySupervisorTime { get; set; }

        public int? ApprovedByAccountingEmployeeID { get; set; }

        public DateTimeOffset? ApprovedByAccountingTime { get; set; }

        public int? ApprovedByBillingEmployeeID { get; set; }

        public DateTimeOffset? ApprovedByBillingTime { get; set; }

        public bool Flagged { get; set; }

        public int? AuthorizeNoteID { get; set; }
        public string AuthorizeNote { get; set; }



    }

    public class ExpandedEmployeeTimerDto : EmployeeTimerDto
    {
        public ICollection<ExpandedEmployeeTimerEntryDto> EmployeeTimerEntries { get; set; }

        public ICollection<EmployeeJobTimerExpandedDto> EmployeeJobTimers { get; set; }

        public ICollection<OccurrenceDto> Occurrences { get; set; }

        public ICollection<NoteDto> Notes { get; set; }

        public BaseEquipmentDto Equipment { get; set; }

        public SimpleEmployeeDto Employee { get; set; }

        public BaseEmployeeDto Supervisor { get; set; }

        public EmployeeTimecardDto EmployeeTimecard { get; set; }
    }
}