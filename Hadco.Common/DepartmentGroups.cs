using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common.Enums;

namespace Hadco.Common
{
    public static class DepartmentGroups
    {
        public static bool EmployeeTimerRequiresTimesheet(int departmentID)
        {
            return _requiresTimesheetID.Contains(departmentID);
        }

        public static bool SupervisorMustBeSelf(int departmentID)
        {
            return _timesheetEmployeeMustBeSelf.Contains(departmentID);
        }

        private static readonly int[] _requiresTimesheetID =
        {
            (int) DepartmentName.Concrete,
            (int) DepartmentName.Development,
            (int) DepartmentName.Residential,
            (int) DepartmentName.Trucking,
            (int) DepartmentName.Mechanic,
            (int) DepartmentName.Transport,
            (int) DepartmentName.Concrete2H,
            (int) DepartmentName.ConcreteHB
        };

        private static readonly int[] _timesheetEmployeeMustBeSelf =
        {
            (int) DepartmentName.Trucking,
            (int) DepartmentName.Mechanic,
            (int) DepartmentName.Transport,
        };
    }
}
