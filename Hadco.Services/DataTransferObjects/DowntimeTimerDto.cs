using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;

namespace Hadco.Services.DataTransferObjects
{
    public class DowntimeTimerDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "DowntimeTimerID")]
        public int ID { get; set; }

        public int DowntimeReasonID { get; set; }

        [JsonConverter(typeof(TruncateSeconds))]
        public DateTimeOffset? StartTime { get; set; }

        [JsonConverter(typeof(TruncateSeconds))]
        public DateTimeOffset? StopTime { get; set; }

        public int TotalMinutes => (StopTime - StartTime).WholeTotalMinutes();

        public int? LoadTimerID { get; set; }
        
        public int? TimesheetID { get; set; }

        public decimal? StartTimeLatitude { get; set; }

        public decimal? StartTimeLongitude { get; set; }

        public decimal? StopTimeLatitude { get; set; }

        public decimal? StopTimeLongitude { get; set; }

        public bool SystemGenerated { get; set; }
    }

    public class DowntimeTimerExpandedDto : DowntimeTimerDto
    {
        public DowntimeReasonDto DowntimeReason { get; set; }
    }
}
