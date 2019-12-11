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
    public class BaseJobDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "JobID")]
        public int ID { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(128)]
        public string JobNumber { get; set; }
    }

    public class JobDto : BaseJobDto
    {
        [MaxLength(128)]
        public string CustomerName { get; set; }

        [MaxLength(128)]
        public string Address1 { get; set; }

        [MaxLength(128)]
        public string City { get; set; }

        [MaxLength(128)]
        public string State { get; set; }

        [MaxLength(128)]
        public string Zip { get; set; }

        [JsonConverter(typeof(DayConverter))]
        public DateTime? DateOpen { get; set; }

        public EntityStatus Status { get; set; }

        public string Memo { get; set; }

        [JsonConverter(typeof(DayConverter))]
        public DateTime? DateFiled { get; set; }

        public int? CustomerTypeID { get; set; }
    }
}
