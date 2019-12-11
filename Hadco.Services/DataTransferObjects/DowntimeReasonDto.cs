using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class DowntimeReasonDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "DowntimeReasonID")]
        public int ID { get; set; }

        [MaxLength(128)]
        public string Description { get; set; }

        [MaxLength(16)]
        public string Code { get; set; }

        [MaxLength(128)]
        public string JobNumber { get; set; }

        [MaxLength(128)]
        public string PhaseNumber { get; set; }

        [MaxLength(128)]
        public string CategoryNumber { get; set; }

        public bool UseLoadJob { get; set; }

        public bool UseLoadPhase { get; set; }

        public bool UseLoadCategory { get; set; }
        public bool DisplayOnMobile { get; set; }
    }
}
