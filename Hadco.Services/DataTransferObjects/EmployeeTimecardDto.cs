using Hadco.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class EmployeeTimecardPostDto
    {
        public int EmployeeID { get; set; }

        public int DepartmentID { get; set; }

        [JsonConverter(typeof(DayConverter))]
        public DateTime Day { get; set; }
    }

    public class EmployeeTimecardDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "EmployeeTimecardID")]
        public int ID { get; set; }

        public int EmployeeID { get; set; }

        public int DepartmentID { get; set; }

        public int SubDepartmentID { get; set; }

        [JsonConverter(typeof(DayConverter))]
        public DateTime StartOfWeek { get; set; }

    }

    public class ExpandedEmployeeTimecardDto : EmployeeTimecardDto
    {
        public ICollection<EmployeeTimerDto> EmployeeTimers { get; set; }
        public DepartmentDto Department { get; set; }
        public EmployeeDto Employee { get; set; }
    }
}
