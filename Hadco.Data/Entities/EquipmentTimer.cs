using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("EquipmentTimers")]
    public class EquipmentTimer : TrackedEntity, IModel
    {
        [Key]
        [Column("EquipmentTimerID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int TimesheetID { get; set; }
        public Timesheet Timesheet { get; set; }

        public int EquipmentID { get; set; }
        public virtual Equipment Equipment { get; set; }

        public ICollection<EquipmentTimerEntry> EquipmentTimerEntries { get; set; }

        [NotMapped]
        public int? EquipmentTimerEntryID => EquipmentTimerEntries?.FirstOrDefault()?.ID;

        [NotMapped]
        public int? EquipmentServiceTypeID => EquipmentTimerEntries?.FirstOrDefault()?.EquipmentServiceTypeID;

        [NotMapped]
        public EquipmentServiceType EquipmentServiceType => EquipmentTimerEntries?.FirstOrDefault()?.EquipmentServiceType;

        [NotMapped]
        public DateTimeOffset? StartTime => EquipmentTimerEntries?.FirstOrDefault()?.StartTime;

        [NotMapped]
        public DateTimeOffset? StopTime => EquipmentTimerEntries?.FirstOrDefault()?.StopTime;

        [NotMapped]
        public string Diary => EquipmentTimerEntries?.FirstOrDefault()?.Diary;

        [NotMapped]
        public bool? Closed => EquipmentTimerEntries?.FirstOrDefault()?.Closed;
    }
}