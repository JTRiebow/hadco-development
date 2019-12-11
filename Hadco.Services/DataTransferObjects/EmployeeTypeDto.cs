using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class EmployeeTypeDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "EmployeeTypeID")]
        public int ID { get; set; }

        [MaxLength(32)]
        public string Description { get; set; }
    }
}
