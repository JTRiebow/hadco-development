using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("OverheadCodes")]
    public class OverheadCode
    {
        [Column(Order = 0), Key]
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
        [Column(Order = 1), Key]
        public string Type { get; set; }
        
        public string JobNumber { get; set; }
        public string PhaseNumber { get; set; }
        public string CategoryNumber { get; set; }
    }
}
