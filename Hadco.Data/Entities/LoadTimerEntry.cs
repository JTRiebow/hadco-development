using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hadco.Data.Entities
{
    public class LoadTimerEntry : TrackedEntity, IModel
    {
        [Key]
        [Column("LoadTimerEntryID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int LoadTimerID { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }

        public decimal? StartTimeLatitude { get; set; }

        public decimal? StartTimeLongitude { get; set; }

        public decimal? EndTimeLatitude { get; set; }

        public decimal? EndTimeLongitude { get; set; }
    }
}
