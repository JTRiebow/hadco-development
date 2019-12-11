using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Common
{
    public static class TimeSpanExtensions
    {
        public static int WholeTotalMinutes(this TimeSpan? span)
        {
            return (int)Math.Floor(span?.TotalMinutes ?? 0);
        }
    }
}
