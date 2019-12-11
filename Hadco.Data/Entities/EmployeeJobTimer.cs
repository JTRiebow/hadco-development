using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("EmployeeJobTimers")]
    public class EmployeeJobTimer : TrackedEntity, IModel
    {
        [Key]
        [Column("EmployeeJobTimerID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int JobTimerID { get; set; }
        public JobTimer JobTimer { get; set; }

        public int EmployeeTimerID { get; set; }
        public EmployeeTimer EmployeeTimer { get; set; }

        public int EquipmentMinutes { get; set; }
        public int LaborMinutes { get; set; }

        [NotMapped]
        public int TotalAllocatedMinutes
            => LaborMinutes + (EmployeeJobEquipmentTimers?.Sum(x => x.EquipmentMinutes) ?? 0);

        public virtual ICollection<EmployeeJobEquipmentTimer> EmployeeJobEquipmentTimers { get; set; }
    }
}
