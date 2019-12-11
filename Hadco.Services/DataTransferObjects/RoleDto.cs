using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class RoleDto : IDataTransferObject
    {
        [Newtonsoft.Json.JsonProperty("RoleId")]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
