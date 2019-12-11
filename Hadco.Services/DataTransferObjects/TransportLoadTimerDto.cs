using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Hadco.Services.DataTransferObjects
{
    public class TransportLoadTimerDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "LoadTimerID")]
        public int ID { get; set; }

        public int TimesheetID { get; set; }

        public int TruckID { get; set; }

        public int TrailerID { get; set; }

        public int? JobID { get; set; }

        public int? PhaseID { get; set; }
 
        public int? CategoryID { get; set; }
   
        [MaxLength(128)]
        public string StartLocation { get; set; }
        [MaxLength(128)]
        public string EndLocation { get; set; }

        public DateTimeOffset? LoadTime { get; set; }
        public DateTimeOffset? DumpTime { get; set; }

        public decimal? LoadTimeLatitude { get; set; }

        public decimal? LoadTimeLongitude { get; set; }

        public decimal? DumpTimeLatitude { get; set; }

        public decimal? DumpTimeLongitude { get; set; }

        public int? LoadEquipmentID { get; set; }
        [MaxLength(32)]
        public string InvoiceNumber { get; set; }
        public string Note { get; set; }

}
}
