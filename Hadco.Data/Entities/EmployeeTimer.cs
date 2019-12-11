using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;

namespace Hadco.Data.Entities
{
    [Table("EmployeeTimers")]
    public class EmployeeTimer : TrackedEntity, IModel
    {
        [Key]
        [Column("EmployeeTimerID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public Timesheet Timesheet { get; set; }
        public int? TimesheetID { get; set; }

        [Column(TypeName = "Date")]
        public DateTime Day { get; set; }

        public bool Injured { get; set; }

        public int? EquipmentID { get; set; }
        public Equipment Equipment { get; set; }

        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }

        public int? ShopMinutes { get; set; }
        public int? TravelMinutes { get; set; }
        public int? GreaseMinutes { get; set; }
        public int? DailyMinutes { get; set; }

        //TODO remove when all front-end use is switched to Total minutes
        public double TotalHours
            =>
                Math.Round(
                    EmployeeTimerEntries?.Sum(x => x.ClockOut.HasValue ? (x.ClockOut.Value - x.ClockIn).TotalHours : 0) ?? 0,
                    2);
        public int TotalMinutes
            => EmployeeTimerEntries?.Sum(x => (x.ClockOut - x.ClockIn).WholeTotalMinutes()) ?? 0;

        public virtual ICollection<EmployeeTimerEntry> EmployeeTimerEntries { get; set; }

        public ICollection<EmployeeJobTimer> EmployeeJobTimers { get; set; }

        public ICollection<Occurrence> Occurrences { get; set; }

        public ICollection<Location> Locations { get; set; }

        public int EquipmentUseTime { get; set; }

        public bool Submitted { get; set; }

        public int DepartmentID { get; set; }
        public Department Department { get; set; }
        public int SubDepartmentID { get; set; }

        public int EmployeeTimecardID { get; set; }
        public EmployeeTimecard EmployeeTimecard { get; set; }

        public bool Flagged { get; set; }

        public int? AuthorizeNoteID { get; set; }
        public Note AuthorizeNote { get; set; }
    }
}
