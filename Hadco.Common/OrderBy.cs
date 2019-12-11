using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Common
{
    public class OrderBy
    {
        public string Field { get; set; }
        public OrderDirection Direction { get; set; }

        public override string ToString()
        {
            return Field + " " + Direction.ToString();
        }
    }

    public enum OrderDirection
    {
        Desc,
        Asc
    }
}
