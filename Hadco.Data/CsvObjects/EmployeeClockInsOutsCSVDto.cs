using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.CsvObjects
{
    public class EmployeeClockInsOutsCSVDto : CsvDto
    {
        public string Employee { get; set; }

        private DateTimeOffset ClockIn { get; set; }

        public string ClockInDate
        {
            get
            {
                return ClockIn.ToString("M/d/yyyy");
            }
        }

        public string ClockInTime {
            get
            {
                return ClockIn.ToString("h:mm:ss tt");
            }
        }

        private DateTimeOffset ClockOut { get; set; }

        public string ClockOutDate
        {
            get
            {
                return ClockOut.ToString("M/d/yyyy");
            }
        }

        public string ClockOutTime {
            get
            {
                return ClockOut.ToString("h:mm:ss tt");
            }
        }

        public int Hours { get; set; }
        
        public int Minutes { get; set; }
    }
}
