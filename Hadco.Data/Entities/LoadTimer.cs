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
    [Table("LoadTimers")]
    public class LoadTimer : TrackedEntity, IModel
    {
        [Key]
        [Column("LoadTimerID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int TimesheetID { get; set; }
        public Timesheet Timesheet { get; set; }

        public int? TruckID { get; set; }
        public Equipment Truck { get; set; }

        public int? TrailerID { get; set; }
        public Equipment Trailer { get; set; }

        public int? PupID { get; set; }
        public Equipment Pup { get; set; }

        public int? JobID { get; set; }
        public Job Job { get; set; }

        public int? PhaseID { get; set; }
        public Phase Phase { get; set; }

        public int? CategoryID { get; set; }
        public Category Category { get; set; }

        [MaxLength(128)]
        public string StartLocation { get; set; }
        [MaxLength(128)]
        public string EndLocation { get; set; }

        [MaxLength(32)]
        public string TicketNumber { get; set; }

        public decimal? Tons { get; set; }

        public ICollection<LoadTimerEntry> LoadTimerEntries { get; set; }

        [NotMapped]
        public DateTimeOffset? LoadTime => LoadTimerEntries?.Select(x => x.StartTime).Min();

        [NotMapped]
        public DateTimeOffset? DumpTime => LoadTimerEntries?.Select(x => x.EndTime).Max();

        [NotMapped]
        public decimal? LoadTimeLatitude => LoadTimerEntries?.OrderBy(x => x.StartTime).FirstOrDefault()?.StartTimeLatitude;
        [NotMapped]
        public decimal? LoadTimeLongitude => LoadTimerEntries?.OrderBy(x => x.StartTime).FirstOrDefault()?.StartTimeLatitude;
        [NotMapped]
        public decimal? DumpTimeLatitude => LoadTimerEntries?.OrderByDescending(x => x.EndTime).FirstOrDefault()?.EndTimeLatitude;
        [NotMapped]
        public decimal? DumpTimeLongitude => LoadTimerEntries?.OrderByDescending(x => x.EndTime).FirstOrDefault()?.EndTimeLongitude;

        public int? MaterialID { get; set; }
        public Material Material { get; set; }

        public int? LoadEquipmentID { get; set; }
        public Equipment LoadEquipment { get; set; }

        public int? BillTypeID { get; set; }
        public BillType BillType { get; set; }
        [MaxLength(50)]
        public string InvoiceNumber { get; set; }
        public string Note { get; set; }

        public bool IsDumped { get; set; }

        public int? TruckClassificationID { get; set; }
        public decimal? PricePerUnit { get; set; }

        public ICollection<DowntimeTimer> DowntimeTimers { get; set; }
    }
}
