using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("Notes")]
    public class Note : IModel
    {
        [Column("NoteID")]
        public int ID { get; set; }

        public string Description { get; set; }

        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }

        [Column(TypeName = "Date")]
        public DateTime Day { get; set; }

        public int DepartmentID { get; set; }
        public virtual Department Department { get; set; }

        public int? EmployeeTimerEntryID { get; set; }
        public virtual EmployeeTimerEntry EmployeeTimerEntry { get; set; }

        public int NoteTypeID { get; set; }
        public NoteType NoteType { get; set; }

        public bool Resolved { get; set; } = false;
        public DateTimeOffset? ResolvedTime { get; set; }

        public int? ResolvedEmployeeID { get; set; }
        public virtual Employee ResolvedEmployee { get; set; }


        public DateTimeOffset? CreatedTime { get; set; }
        public int? CreatedEmployeeID { get; set; }
        public virtual Employee CreatedEmployee { get; set; }
        public DateTimeOffset ModifiedTime { get; set; } = DateTimeOffset.Now;
    }
}
