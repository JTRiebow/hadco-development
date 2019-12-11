using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class EmployeeJobEquipmentTimerDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "EmployeeJobEquipmentTimerID")]        
        public int ID { get; set; }

        public int? EmployeeJobTimerID { get; set; }

        public int? EmployeeTimerID { get; set; }

        public int? JobTimerID { get; set; }

        public int EquipmentID { get; set; }

        public int EquipmentMinutes { get; set; }

        public decimal EquipmentHours => Math.Round(EquipmentMinutes / (decimal)60, 2);
    }

    public class EmployeeJobEquipmentTimerExpandedDto : EmployeeJobEquipmentTimerDto
    {
        public BaseEquipmentDto Equipment { get; set; }

    }

}
