using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Hadco.Services.DataTransferObjects
{
    public class LocationDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "LocationID")]
        public int ID { get; set; }

        public int EmployeeTimerID { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}
