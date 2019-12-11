using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services
{
    public enum Action
    {        
        Read = 1,
        Write = 2,
        Delete = 4,
        Options = 8
    }
}
