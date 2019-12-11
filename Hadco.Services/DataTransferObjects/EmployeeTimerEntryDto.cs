using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;

namespace Hadco.Services.DataTransferObjects
{
    public class EmployeeTimerEntryDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "EmployeeTimerEntryID")]
        public int ID { get; set; }

        public int EmployeeTimerID { get; set; }

        [JsonConverter(typeof(TruncateSeconds))]
        public DateTimeOffset ClockIn { get; set; }

        [JsonConverter(typeof(TruncateSeconds))]
        public DateTimeOffset? ClockOut { get; set; }

        public int? ClockInEmployeeID { get; set; }

        public int? ClockOutEmployeeID { get; set; }

        public string ClockInNote { get; set; }
     
        public string ClockOutNote { get; set; }

        public decimal? ClockInLatitude { get; set; }

        public decimal? ClockInLongitude { get; set; }

        public decimal? ClockOutLatitude { get; set; }

        public decimal? ClockOutLongitude { get; set; }

        public int? PitID { get; set; }

    }

    public class ExpandedEmployeeTimerEntryDto : EmployeeTimerEntryDto
    {        

        public ICollection<EmployeeTimerEntryHistoryDto> EmployeeTimerEntryHistories { get; set; }
    }
}
