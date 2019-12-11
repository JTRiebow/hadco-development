using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Hadco.Common.DataTransferObjects
{
    public class PriceListDto
    {
        public int PricingID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Job { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Customer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Run { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? StartDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset EndDate { get; set; }

    }
}
