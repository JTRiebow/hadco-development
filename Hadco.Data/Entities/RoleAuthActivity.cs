using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;
using Hadco.Common.Enums;

namespace Hadco.Data.Entities
{
    [Table("RoleAuthActivities")]
    public class RoleAuthActivity : TrackedEntity, IModel
    {

        [Key]
        [Column("RoleAuthActivityID")]
        public int ID { get; set; }

        [Index("UX_RoleID_AuthActivityID", 0, IsUnique = true)]
        public int RoleID { get; set; }
        public Role Role { get; set; }

        [Index("UX_RoleID_AuthActivityID", 1, IsUnique = true)]
        public int AuthActivityID { get; set; }
        public AuthActivity AuthActivity { get; set; }

        public bool OwnDepartments { get; set; }
        public bool AllDepartments { get; set; }
        public ICollection<Department> Departments { get; set; }

    }
}
