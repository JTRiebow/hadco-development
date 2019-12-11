using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("EmployeeTimerEntries")]
    public class EmployeeTimerEntry : TrackedEntity, IModel
    {
        [Key]
        [Column("EmployeeTimerEntryID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int EmployeeTimerID { get; set; }

        public EmployeeTimer EmployeeTimer { get; set; }

        public DateTimeOffset ClockIn { get; set; }

        public DateTimeOffset? ClockOut { get; set; }

        public int? ClockInEmployeeID { get; set; }

        public Employee ClockInEmployee { get; set; }

        public int? ClockOutEmployeeID { get; set; }
        public Employee ClockOutEmployee { get; set; }

        public int? ClockInNoteID { get; set; }
        public virtual Note ClockInNote { get; set; } 

        public int? ClockOutNoteID { get; set; }
        public virtual Note ClockOutNote { get; set; }

        public decimal? ClockInLatitude { get; set; }

        public decimal? ClockInLongitude { get; set; }

        public decimal? ClockOutLatitude { get; set; }

        public decimal? ClockOutLongitude { get; set; }

        public ICollection<EmployeeTimerEntryHistory> EmployeeTimerEntryHistories { get; set; }

        public int? PitID { get; set; }
        public Pit Pit { get; set; }
    }
}
