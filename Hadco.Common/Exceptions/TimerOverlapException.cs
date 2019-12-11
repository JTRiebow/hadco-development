using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Common.Exceptions
{
    public class TimerOverlapException : Exception
    {
        public TimerOverlapException(string message) 
            : base(message)
        { }
    }
}
