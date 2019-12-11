using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Hadco.Common.DataTransferObjects
{
    public class TruckerDailyDto
    {
        public int LoadTimerID { get; set; }
        public int? DowntimeTimerID { get; set; }
        public string BillType { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Truck { get; set; }
        public string Trailer { get; set; }
        public string PupTrailer { get; set; }
        public string TicketNumber { get; set; }
        public string DowntimeCode { get; set; }
        public string Tons { get; set; }
        public string LoadSite { get; set; }
        public string DumpSite { get; set; }
        public string Material { get; set; }
        public string LoadEquipment { get; set; }
        public string Job { get; set; }
        public int? JobID { get; set; }
        public string Phase { get; set; }
        public int? PhaseID { get; set; }
        public string Category { get; set; }
        public int? CategoryID { get; set; }
        public decimal? TotalHours { get; set; }
        public string InvoiceNumber { get; set; }
        public string Note { get; set; }
        public string Location { get; set; }
        public int DepartmentID { get; set; }
        public decimal? PricePerUnit { get; set; }
        public decimal? CalculatedRevenue { get; set; }
    }
}
