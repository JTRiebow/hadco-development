using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Hadco.Services.DataTransferObjects
{
    public class LoadTimerEntryDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "LoadTimerEntryID")]
        public int ID { get; set; }
        public int LoadTimerID { get; set; }

        [JsonConverter(typeof(TruncateSeconds))]
        public DateTimeOffset? StartTime { get; set; }

        [JsonConverter(typeof(TruncateSeconds))]
        public DateTimeOffset? EndTime { get; set; }

        public decimal? StartTimeLatitude { get; set; }

        public decimal? StartTimeLongitude { get; set; }

        public decimal? EndTimeLatitude { get; set; }

        public decimal? EndTimeLongitude { get; set; }
    }
}
