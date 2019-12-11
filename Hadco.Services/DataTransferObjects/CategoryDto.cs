using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class CategoryDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "CategoryID")]
        public int ID { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(128)]
        public string CategoryNumber { get; set; }

        public int PhaseID { get; set; }

        public int JobID { get; set; }

        public string UnitsOfMeasure { get; set; }

        public decimal? PlannedQuantity { get; set; }
    }
}
