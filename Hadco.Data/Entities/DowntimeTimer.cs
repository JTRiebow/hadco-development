using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("DowntimeTimers")]
    public class DowntimeTimer : TrackedEntity, IModel
    {
        [Key]
        [Column("DowntimeTimerID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int DowntimeReasonID { get; set; }
        public DowntimeReason DowntimeReason { get; set; }

        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? StopTime { get; set; }

        public int? LoadTimerID { get; set; }
        public LoadTimer LoadTimer { get; set; }

        public int? TimesheetID { get; set; }
        public Timesheet Timesheet { get; set; }

        public decimal? StartTimeLatitude { get; set; }

        public decimal? StartTimeLongitude { get; set; }

        public decimal? StopTimeLatitude { get; set; }

        public decimal? StopTimeLongitude { get; set; }

        public bool SystemGenerated { get; set; }
    }
}
