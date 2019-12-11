using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("EmployeeTimerEntryHistories")]
    public class EmployeeTimerEntryHistory : IModel
    {
        [Key]
        [Column("EmployeeTimerEntryHistoryID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        public int EmployeeTimerEntryID { get; set; }
        public EmployeeTimerEntry EmployeeTimerEntry { get; set; }

        public DateTimeOffset? PreviousClockIn { get; set; }

        public DateTimeOffset? PreviousClockOut { get; set; }

        public DateTimeOffset? CurrentClockIn { get; set; }

        public DateTimeOffset? CurrentClockOut { get; set; }

        public int ChangedByID { get; set; }
        public Employee ChangedBy { get; set; }

        public DateTimeOffset ChangedTime { get; set; }
    }
}
