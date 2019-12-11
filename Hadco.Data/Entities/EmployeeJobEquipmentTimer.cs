using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("EmployeeJobEquipmentTimers")]
    public class EmployeeJobEquipmentTimer : TrackedEntity, IModel
    {
        [Key]
        [Column("EmployeeJobEquipmentTimerID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int EmployeeJobTimerID { get; set; }
        public EmployeeJobTimer EmployeeJobTimer { get; set; }

        public int EquipmentID { get; set; }
        public Equipment Equipment { get; set; }

        public int EquipmentMinutes { get; set; }
    }
}
