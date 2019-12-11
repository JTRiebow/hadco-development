using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class BasePriceDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "PriceID")]
        public int ID { get; set; }

        public decimal Value { get; set; }
    }

    public class PriceDto : BasePriceDto
    {

        [Required]
        public int PricingID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaterialID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? TruckClassificationID { get; set; }
    }

    public class ExpandedPriceDto : PriceDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Material  { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Truck  { get; set; }
    }
}
