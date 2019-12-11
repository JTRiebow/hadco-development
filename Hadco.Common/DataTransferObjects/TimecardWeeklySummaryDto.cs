using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Common.DataTransferObjects
{
    public class TimecardWeeklySummaryDto
    {
        public int EmployeeTimecardID { get; set; }
        public int DepartmentID { get; set; }
        public int EmployeeID { get; set; }
        public string StartOfWeek { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Supervisor { get; set; }
        public EmployeeTimerSummary Day0 { get; set; }
        public EmployeeTimerSummary Day1 { get; set; }
        public EmployeeTimerSummary Day2 { get; set; }
        public EmployeeTimerSummary Day3 { get; set; }
        public EmployeeTimerSummary Day4 { get; set; }
        public EmployeeTimerSummary Day5 { get; set; }
        public EmployeeTimerSummary Day6 { get; set; }
        public string Total { get; set; }
    }

    public class EmployeeTimerSummary
    {
        [JsonIgnore]
        public int EmployeeTimecardID { get; set; }

        [JsonIgnore]
        public int DayNumber { get; set; }

        public decimal TotalHours { get; set; }

        public bool ApprovedBySupervisor { get; set; }

        public bool ApprovedByAccounting { get; set; }

        public bool ApprovedByBilling { get; set; }

        public bool SystemFlagged { get; set; }

        public bool UserFlagged { get; set; }

        public bool Injured { get; set; }

        public bool HasOccurrence { get; set; }
    }
}
