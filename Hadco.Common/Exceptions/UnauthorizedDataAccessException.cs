using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Common
{
    public class UnauthorizedDataAccessException : Exception
    {
        public UnauthorizedDataAccessException()
            :base()
        { }

        public UnauthorizedDataAccessException(string message)
            : base(message)
        { }
    }
}
