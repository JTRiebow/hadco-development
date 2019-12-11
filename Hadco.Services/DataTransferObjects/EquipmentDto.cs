using Hadco.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class BaseEquipmentDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "EquipmentID")]
        public int ID { get; set; }

        [MaxLength(128)]
        public string EquipmentNumber { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }
    }

    public class EquipmentDto : BaseEquipmentDto
    {
        [MaxLength(128)]
        public string Model { get; set; }

        [MaxLength(128)]
        public string Fleetcode { get; set; }

        [MaxLength(128)]
        public string SerialNumber { get; set; }

        public EntityStatus Status { get; set; }

        public string Type { get; set; }
    }


}
