using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Common.DataTransferObjects
{
    public class EmployeeJobTimerSummaryDto
    {
        public int EmployeeJobTimerID { get; set; }
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public int LaborMinutes { get; set; }

        public IEnumerable<EmployeeJobEquipmentTimerSummaryDto> EmployeeJobEquipmentTimers { get; set; }
    }

    public class EmployeeJobEquipmentTimerSummaryDto
    {
        public int EmployeeJobEquipmentTimerID { get; set; }
        public int EquipmentID { get; set; }
        public int EquipmentMinutes { get; set; }
        public string EquipmentNumber { get; set; }
        public int EmployeeJobTimerID { get; set; }
    }
}
