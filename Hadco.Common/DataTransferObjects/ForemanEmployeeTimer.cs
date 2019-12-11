using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hadco.Common.DataTransferObjects
{
    public class ForemanTimesheet
    {
        public int TimesheetID { get; set; }
        public IEnumerable<ForemanEmployeeTimer> EmployeeTimers { get; set; }     
    }

    public class ForemanEmployeeTimerPatch
    {
        [JsonProperty("EmployeeTimerID")]
        public int ID { get; set; }
        public bool Injured { get; set; }
        public bool Submitted { get; set; }
        public int? ShopMinutes { get; set; }
        public int? TravelMinutes { get; set; }
        public int? GreaseMinutes { get; set; }
        public int? DailyMinutes { get; set; }
        public int EmployeeID { get; set; }
        public int DepartmentID { get; set; }
        public int SubDepartmentID { get; set; }
        public int? EmployeeTimecardID { get; set; }
        public int TimesheetID { get; set; }
        public DateTime Day { get; set; }
        public IEnumerable<ForemanEmployeeJobTimerPatch> EmployeeJobTimers { get; set; }
    }
    public class ForemanEmployeeTimer : ForemanEmployeeTimerPatch
    {
        public string EmployeeName { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public int TotalMinutes { get; set; }

        public bool HasNote { get; set; }
        public bool ApprovedBySupervisor { get; set; } = false;
        public bool ApprovedByBilling { get; set; } = false;
        public bool ApprovedByAccounting { get; set; } = false;

        public new IEnumerable<ForemanEmployeeJobTimer> EmployeeJobTimers { get; set; }
        public IEnumerable<EmployeeTimerOccurrence> Occurrences { get; set; }
    }

    public class ForemanEmployeeJobTimerPatch
    {
        [JsonProperty("EmployeeJobTimerID")]
        public int? ID { get; set; }
        public int JobTimerID { get; set; }

        public int EmployeeTimerID { get; set; }

        public int? LaborMinutes { get; set; }

        public IEnumerable<ForemanEmployeeJobEquipmentTimerPatch> EmployeeJobEquipmentTimers { get; set; }
    }
    public class ForemanEmployeeJobTimer : ForemanEmployeeJobTimerPatch
    {
        public string JobPhaseCategory { get; set; }
        public int? JobID { get; set; }
        public int? PhaseID { get; set; }
        public int? CategoryID { get; set; }
        public string JobNumber { get; set; }
        public string PhaseNumber { get; set; }
        public string CategoryNumber { get; set; }
        public string Diary { get; set; }
        public string UnitsOfMeasure { get; set; }
        public decimal? PlannedQuantity { get; set; }
        public decimal? PreviousQuantity { get; set; }
        public decimal? NewQuantity { get; set; }
        public decimal? OtherNewQuantity { get; set; }
        public string InvoiceNumber { get; set; }

        public new IEnumerable<ForemanEmployeeJobEquipmentTimer> EmployeeJobEquipmentTimers { get; set; }
    }

    public class ForemanEmployeeJobEquipmentTimerPatch
    {
        [JsonProperty("EmployeeJobEquipmentTimerID")]
        public int? ID { get; set; }
        public int? EmployeeJobTimerID { get; set; }

        public int EquipmentID { get; set; }

        public int? EquipmentMinutes { get; set; }
    }

    public class ForemanEmployeeJobEquipmentTimer : ForemanEmployeeJobEquipmentTimerPatch
    {
        public int JobTimerID { get; set; }
        public int EmployeeTimerID { get; set; }
        public string EquipmentNumber { get; set; }
    }

    public class EmployeeTimerOccurrence
    {
        [JsonIgnore]
        public int EmployeeTimerID { get; set; }
        public int OccurrenceID { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }

}
