using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("EmployeeTimecards")]
    public class EmployeeTimecard : TrackedEntity, IModel
    {
        [Key]
        [Column("EmployeeTimecardID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int EmployeeID { get; set; }

        public Employee Employee { get; set; }

        public int DepartmentID { get; set; }

        public int SubDepartmentID { get; set; }

        public Department Department { get; set; }

        [Column(TypeName = "Date")]
        public DateTime StartOfWeek { get; set; }

        public ICollection<EmployeeTimer> EmployeeTimers { get; set; }

    }
}
