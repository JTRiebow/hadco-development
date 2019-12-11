using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.CsvObjects
{
    public class EmployeeRolesCsvDto : CsvDto
    {
        public string Name { get; set; }

        public string Username { get; set; }

        public string Departments { get; set; }

        public string Supervisors { get; set; }

        public string Roles { get; set; }

        public string AllDepartmentsPermissions { get; set; }

        public string OwnDepartmentsPermissions { get; set; }

        public string OwnDepartments { get; set; }

        public string SpecificDepartmentsPermissions { get; set; }

        public string SpecificDepartments { get; set; }
    }
}
