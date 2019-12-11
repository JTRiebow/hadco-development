using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common.Enums;

namespace Hadco.Services.DataTransferObjects
{
    public class NoteDto : IDataTransferObject
    {
        [JsonProperty("NoteID")]
        public int ID { get; set; }

        public int EmployeeID { get; set; }

        public DateTime Day { get; set; }

        public int DepartmentID { get; set; }

        public string Description { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public int? CreatedEmployeeID { get; set; }
        public virtual BaseEmployeeDto CreatedEmployee { get; set; }


        public bool Resolved { get; set; }
        public DateTimeOffset? ResolvedTime { get; set; }
        public int? ResolvedEmployeeID { get; set; }
        public BaseEmployeeDto ResolvedEmployee { get; set; }

        public NoteTypeName NoteTypeID { get; set; }
        public NoteTypeDto NoteType { get; set; }

        public DateTimeOffset ModifiedTime { get; set; }
    }
}
