using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class CustomerDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "CustomerID")]
        public int ID { get; set; }

        [MaxLength(128)]
        public string CustomerNumber { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

    }
}
