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
    public class EmployeeTimerEntryHistoryDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "EmployeeTimerEntryHistoryID")]
        public int ID { get; set; }

        public int EmployeeTimerEntryID { get; set; }

        public DateTimeOffset? PreviousClockIn { get; set; }

        public DateTimeOffset? PreviousClockOut { get; set; }

        public DateTimeOffset? CurrentClockIn { get; set; }

        public DateTimeOffset? CurrentClockOut { get; set; }

        public int ChangedByID { get; set; }
        public SimpleEmployeeDto ChangedBy { get; set; }

        public DateTimeOffset ChangedTime { get; set; }
    }    
}
