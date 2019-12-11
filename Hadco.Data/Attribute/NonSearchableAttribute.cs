using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NonSearchableAttribute : System.Attribute
    {
    }
}
