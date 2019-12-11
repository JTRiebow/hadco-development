using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;

namespace Hadco.Services.DataTransferObjects
{
    public class LoadTimerDto : IDataTransferObject
    {
        [JsonProperty(PropertyName = "LoadTimerID")]
        public int ID { get; set; }

        public int TimesheetID { get; set; }

        public int? TruckID { get; set; }

        public int? TrailerID { get; set; }

        public int? PupID { get; set; }

        public int? JobID { get; set; }

        public int? PhaseID { get; set; }

        public int? CategoryID { get; set; }

        [MaxLength(128)]
        public string StartLocation { get; set; }

        [MaxLength(128)]
        public string EndLocation { get; set; }

        [MaxLength(32)]
        public string TicketNumber { get; set; }

        public decimal? Tons { get; set; }

        public DateTimeOffset? LoadTime { get; set; }

        public DateTimeOffset? DumpTime { get; set; }

        public decimal? LoadTimeLatitude { get; set; }

        public decimal? LoadTimeLongitude { get; set; }

        public decimal? DumpTimeLatitude { get; set; }

        public decimal? DumpTimeLongitude { get; set; }

        public ICollection<LoadTimerEntryDto> LoadTimerEntries { get; set; }

        public decimal? TotalHours
        {
            get
            {
                var entires = LoadTimerEntries?.Where(x => x.StartTime.HasValue && x.EndTime.HasValue).ToList();
                if (entires == null || !entires.Any())
                {
                    return null;
                }
                var hours = entires.Sum(x => (x.EndTime - x.StartTime).Value.TotalHours);
                return decimal.Round(Convert.ToDecimal(hours), 2);
            }
        }

        public int? TotalMinutes => LoadTimerEntries?.Sum(x => (x.EndTime - x.StartTime).WholeTotalMinutes()) ?? 0;
        public int? MaterialID { get; set; }

        public int? LoadEquipmentID { get; set; }

        public int? BillTypeID { get; set; }
        [MaxLength(50)]
        public string InvoiceNumber { get; set; }
        public string Note { get; set; }
        public bool IsDumped { get; set; }
        public int? TruckClassificationID { get; set; }
        public decimal? PricePerUnit { get; set; }
    }

    public class LoadTimerExpandedDto : LoadTimerDto
    {
        public BaseEquipmentDto Truck { get; set; }

        public BaseEquipmentDto Trailer { get; set; }

        public BaseEquipmentDto Pup { get; set; }

        public JobDto Job { get; set; }

        public PhaseDto Phase { get; set; }

        public CategoryDto Category { get; set; }

        public MaterialDto Material { get; set; }

        public BaseEquipmentDto LoadEquipment { get; set; }

        public BillTypeDto BillType { get; set; }

        public ICollection<DowntimeTimerExpandedDto> DowntimeTimers { get; set; }

    }

    public class LoadTimerPrimaryDto : LoadTimerExpandedDto
    {
        public TimesheetDto Timesheet { get; set; }
    }
}
