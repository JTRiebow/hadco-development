using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class DailyApprovalPatchDto 
    {
        public bool ApprovedBySupervisor { get; set; }
        public bool ApprovedByBilling { get; set; }
        public bool ApprovedByAccounting { get; set; }
    }

    public class BaseDailyApprovalDto : DailyApprovalPatchDto
    {
        public int EmployeeID { get; set; }
        public DateTime Day { get; set; }
        public int DepartmentID { get; set; }
    }

    public class DailyApprovalDto : BaseDailyApprovalDto, IDataTransferObject
    {
        [JsonProperty("DailyApprovalID")]
        public int ID { get; set; }

        public int? ApprovedBySupervisorEmployeeID { get; set; }
        public DateTimeOffset? ApprovedBySupervisorTime { get; set; }

        public int? ApprovedByBillingEmployeeID { get; set; }
        public DateTimeOffset? ApprovedByBillingTime { get; set; }

        public int? ApprovedByAccountingEmployeeID { get; set; }
        public DateTimeOffset? ApprovedByAccountingTime { get; set; }
    }
}
