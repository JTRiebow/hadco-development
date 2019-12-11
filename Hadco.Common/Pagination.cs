using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Common
{
    public class Pagination
    {
        public Pagination()
        {
            Skip = 0;
            Take = int.MaxValue;
            OrderBy = null;
        }
        public int Skip { get; set; }
        public int Take { get; set; }
        public IEnumerable<OrderBy> OrderBy { get; set; }
    }
}
