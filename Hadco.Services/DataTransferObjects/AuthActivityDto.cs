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
    public class AuthActivityDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "AuthActivityID"), EnumDataType(typeof(AuthActivityID))]
        public int ID { get; set; }

        public AuthSectionID AuthSectionID { get; set; }
        public AuthSectionDto AuthSection { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
