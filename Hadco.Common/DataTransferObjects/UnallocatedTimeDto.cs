using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Common.DataTransferObjects
{
    public class UnallocatedTimeDto
    {
        public string Name { get; set; }

        public string Day { get; set; }

        public decimal TotalHours { get; set; }

        public decimal AllocatedHours { get; set; }

        public decimal SecondsDifference { get; set; }

        public decimal HoursDifference { get; set; }
    }

    public class UnallocatedTimeConcreteDto : UnallocatedTimeDto
    {
        public decimal AllocatedLaborHours { get; set; }

        public decimal AllocatedEquipmentHours { get; set; }
    }
}
