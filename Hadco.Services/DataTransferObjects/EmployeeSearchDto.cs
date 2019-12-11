using System.Collections.Generic;
using Hadco.Common.DataTransferObjects;

namespace Hadco.Services.DataTransferObjects
{
    public class EmployeeSearchDto
    {
        public ExpandedEmployeeDto Employee { get; set; }
        public IEnumerable<TimecardWeeklySummaryDto> Timers { get; set; }
        public IEnumerable<SuperintendentTimesheetsDto> Timesheets { get; set; }
        public bool IsBillingVisible { get; set; }
        public bool IsAccountingVisible { get; set; }
        public bool IsSupervisorVisible { get; set; }
    }
}
