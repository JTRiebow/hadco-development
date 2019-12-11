using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("DailyApprovals")]
    public class DailyApproval : TrackedEntity, IModel
    {
        [Key, Column("DailyApprovalID")]
        public int ID { get; set; }

        [Index("IX_EmployeeDayDepartmentApproval", 1, IsUnique = true)]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }

        [Column(TypeName = "Date")]
        [Index("IX_EmployeeDayDepartmentApproval", 2, IsUnique = true)]
        public DateTime Day { get; set; }

        [Index("IX_EmployeeDayDepartmentApproval", 3, IsUnique = true)]
        public int DepartmentID { get; set; }
        public Department Department { get; set; }

        public bool ApprovedBySupervisor { get; set; } = false;
        public int? ApprovedBySupervisorEmployeeID { get; set; }
        public Employee ApprovedBySupervisorEmployee { get; set; }
        public DateTimeOffset? ApprovedBySupervisorTime { get; set; }

        public bool ApprovedByBilling { get; set; } = false;
        public int? ApprovedByBillingEmployeeID { get; set; }
        public Employee ApprovedByBillingEmployee { get; set; }
        public DateTimeOffset? ApprovedByBillingTime { get; set; }

        public bool ApprovedByAccounting { get; set; } = false;
        public int? ApprovedByAccountingEmployeeID { get; set; }
        public Employee ApprovedByAccountingEmployee { get; set; }
        public DateTimeOffset? ApprovedByAccountingTime { get; set; }
    }
}
