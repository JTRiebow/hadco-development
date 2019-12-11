using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;

namespace Hadco.Services.DataTransferObjects
{
    public class EquipmentTimerDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "EquipmentTimerID")]

        public int ID { get; set; }

        public int TimesheetID { get; set; }

        public int EquipmentID { get; set; }

        public ICollection<EquipmentTimerEntryDto> EquipmentTimerEntries { get; set; }

        public double TotalHours 
            => EquipmentTimerEntries != null ? Math.Round(
                EquipmentTimerEntries.Sum(
                    x => (x.StopTime - x.StartTime).GetValueOrDefault(TimeSpan.Zero).TotalHours), 2) : 0;

        public int TotalMinutes
            => EquipmentTimerEntries != null ? EquipmentTimerEntries.Sum(x => (x.StopTime - x.StartTime).WholeTotalMinutes()) : 0;

        // These properties actually belong to the Equipment Timer Entries, but have been preserved for backwards compatability with the old app.
        public int? EquipmentTimerEntryID { get; set; }

        public int? EquipmentServiceTypeID { get; set; }

        [JsonConverter(typeof(TruncateSeconds))]
        public DateTimeOffset? StartTime { get; set; }

        [JsonConverter(typeof(TruncateSeconds))]
        public DateTimeOffset? StopTime { get; set; }

        public string Diary { get; set; }

        public bool? Closed { get; set; }
    }


    public class ExpandedEquipmentTimerDto : EquipmentTimerDto
    {
        public EquipmentServiceTypeDto EquipmentServiceType { get; set; }

        public BaseEquipmentDto Equipment { get; set; }
    }
}
