using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    public abstract class TrackedEntity
    {
        public DateTimeOffset? CreatedOn { get; set; }

        public DateTimeOffset? UpdatedOn { get; set; }

        public int? CreatedEmployeeID { get; set; }
        public virtual Employee CreatedEmployee { get; set; }

        public int? ModifiedEmployeeID { get; set; }
    }
}
