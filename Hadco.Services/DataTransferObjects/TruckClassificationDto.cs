using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class TruckClassificationDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "TruckClassificationID")]
        public int ID { get; set; }
        [MaxLength(32)]
        [Required]
        public string Name { get; set; }
        [MaxLength(16)]
        [Required]
        public string Truck { get; set; }
        [MaxLength(16)]
        public string Trailer1 { get; set; }
        [MaxLength(16)]
        public string Trailer2 { get; set; }
        [MaxLength(16)]
        public string Code { get; set; }
    }
}
