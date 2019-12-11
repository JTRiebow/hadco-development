using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common.Enums;

namespace Hadco.Services.DataTransferObjects
{
    public class BillTypeDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "BillTypeID"), EnumDataType(typeof(BillTypeName))]
        public int ID { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }
    }
}
