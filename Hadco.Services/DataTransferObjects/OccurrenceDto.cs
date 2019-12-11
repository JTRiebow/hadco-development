using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class OccurrenceDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "OccurrenceID")]
        public int ID { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }

        [MaxLength(1)]
        public string Code { get; set; }
    }
}
