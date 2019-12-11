using Hadco.Common.DataTransferObjects;
using Hadco.Data.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;

namespace Hadco.Services.DataTransferObjects
{
    public class PricingDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "PricingID")]
        public int ID { get; set; }
        
        [Required]
        public int CustomerTypeID { get; set; }

        [Required]
        public int BillTypeID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? PhaseID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? JobID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? CustomerID { get; set; }

        [JsonConverter(typeof(DayConverter))]
        public DateTime StartDate { get; set; }
        [JsonConverter(typeof(DayConverter))]
        public DateTime? EndDate { get; set; }

        public DateTimeOffset UpdatedTime { get; set; }

       // [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool CanOverlap { get; set; }

    }

    public class ExpandedPricingDto : PricingDto
    {

        [JsonIgnore]
        public Phase Phase { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PhaseNumber
        {
            get
            {
                return Phase?.PhaseNumber;
            }
        }

        [JsonIgnore]
        public Job Job { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JobNumber
        {
            get
            {
                return Job?.JobNumber;
            }
        }

        [JsonIgnore]
        public Customer Customer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CustomerNumber
        {
            get
            {
                return Customer?.CustomerNumber;
            }
        }

        public ICollection<ExpandedPriceDto> Prices { get; set; }

    }
}
