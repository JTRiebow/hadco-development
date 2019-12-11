using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Common.DataTransferObjects
{
    public class SuperintendentTimesheetsDto
    {

        public int EmployeeID { get; set; }

        public string Name { get; set; }

        public int DepartmentID { get; set; }

        public IEnumerable<ForemanTimesheetDto> Timesheets { get; set; }
    }

    public class ForemanTimesheetDto
    {
        public int TimesheetID { get; set; }
        public int EmployeeID { get; set; }

        public  int DepartmentID { get; set; }

        public DateTime Day { get; set; }

        public IEnumerable<string> JobNumbers { get; set; }
    }
}
