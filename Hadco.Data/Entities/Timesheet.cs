using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("Timesheets")]
    public class Timesheet : TrackedEntity, IModel
    {
        [Key]
        [Column("TimesheetID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        ///     The equipment that was used for the day
        /// </summary>
        public int? EquipmentID { get; set; }
        public Equipment Equipment { get; set; }

        [Index("IX_DepartmentIDEmployeeIDAndDay", 1, IsUnique = true)]
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
        
        /// <summary>
        ///     The time in minutes that the equipment was used.
        /// </summary>
        public int EquipmentUseTime { get; set; }

        [Index("IX_DepartmentIDEmployeeIDAndDay", 2, IsUnique = true)]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }

        [Index("IX_DepartmentIDEmployeeIDAndDay", 3, IsUnique = true)]
        [Column(TypeName = "Date")]
        public DateTime Day { get; set; }

        public ICollection<EmployeeTimer> EmployeeTimers { get; set; }

        public ICollection<EquipmentTimer> EquipmentTimers { get; set; }

        public ICollection<JobTimer> JobTimers { get; set; }

        public ICollection<LoadTimer> LoadTimers { get; set; }

        public ICollection<DowntimeTimer> DowntimeTimers { get; set; }
        
        public int? Odometer { get; set; }

        public string Notes { get; set; }

        public DateTimeOffset? PreTripStart { get; set; }
        public DateTimeOffset? PreTripEnd { get; set; }
        public DateTimeOffset? DepartureTime { get; set; }
        public DateTimeOffset? ArrivalAtJob { get; set; }
        public DateTimeOffset? PostJob { get; set; }
        public DateTimeOffset? PostJobEnd { get; set; }
        
    }
}
