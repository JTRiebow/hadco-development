using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("JobTimers")]
    public class JobTimer : TrackedEntity, IModel
    {
        [Key]
        [Column("JobTimerID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int TimesheetID { get; set; }
        public Timesheet Timesheet { get; set; }

        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? StopTime { get; set; }

        public string Diary { get; set; }

        [MaxLength(50)]
        public string InvoiceNumber { get; set; }

        public decimal? NewQuantity { get; set; }

        public int JobID { get; set; }
        public Job Job { get; set; }
        public int PhaseID { get; set; }
        public Phase Phase { get; set; }
        public int CategoryID { get; set; }
        public Category Category { get; set; }

        public ICollection<EmployeeJobTimer> EmployeeJobTimers { get; set; }


    }
}
