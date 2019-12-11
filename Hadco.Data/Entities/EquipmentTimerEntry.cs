using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("EquipmentTimerEntries")]
    public class EquipmentTimerEntry : TrackedEntity, IModel
    {
        [Key]
        [Column("EquipmentTimerEntryID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int EquipmentTimerID { get; set; }
        public virtual EquipmentTimer EquipmentTimer { get; set; }

        public int EquipmentServiceTypeID { get; set; }
        public virtual EquipmentServiceType EquipmentServiceType { get; set; }

        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? StopTime { get; set; }

        public string Diary { get; set; }
        public bool? Closed { get; set; }
    }
}
