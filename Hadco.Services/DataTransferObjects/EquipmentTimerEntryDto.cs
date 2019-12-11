using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;
using Newtonsoft.Json;

namespace Hadco.Services.DataTransferObjects
{
    public class EquipmentTimerEntryDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "EquipmentTimerEntryID")]
        public int ID { get; set; }

        public int EquipmentTimerID { get; set; }

        public int EquipmentServiceTypeID { get; set; }

        [JsonConverter(typeof(TruncateSeconds))]
        public DateTimeOffset? StartTime { get; set; }

        [JsonConverter(typeof(TruncateSeconds))]
        public DateTimeOffset? StopTime { get; set; }

        public double? TotalHours
        {
            get
            {
                var hours = (StopTime - StartTime)?.TotalHours;
                return hours.HasValue ? (double?)Math.Round(hours.Value, 2) : null;
            }
        }

        public int? TotalMinutes => (StopTime - StartTime).WholeTotalMinutes();

        public string Diary { get; set; }
        public bool? Closed { get; set; }
    }

    public class ExpandedEquipmentTimerEntryDto : EquipmentTimerEntryDto
    {

        public int TimesheetID { get; set; }

        public int EquipmentID { get; set; }

        public BaseEquipmentDto Equipment { get; set; }

        public EquipmentServiceTypeDto EquipmentServiceType { get; set; }

    }
}

