using Hadco.Services.DataTransferObjects;
using Newtonsoft.Json;

namespace Hadco.Services.DataTransferObjects
{
    public class NoteTypeDto :IDataTransferObject
    {
        [JsonProperty("NoteTypeID")]
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsSystemGenerated { get; set; }
    }
}