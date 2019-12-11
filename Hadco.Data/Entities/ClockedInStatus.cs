using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace Hadco.Data.Entities
{
    public class ClockedInStatus
    {
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }
        public bool IsClockedIn { get; set; }
    }
}
