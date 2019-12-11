using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Common
{
    public static class EnumHelper
    {
        public static IDictionary<string, object> ToDictionary<T>() where T : struct, IConvertible
             
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            var values = Enum.GetValues(typeof(T));
            var dictionary = new Dictionary<string, object>();
            foreach (var value in values)
            {
                dictionary.Add(value.ToString(), (int)value);
                dictionary.Add(((int)value).ToString(), value.ToString().Substring(0, 1).ToLower() + value.ToString().Substring(1));
            }
            return dictionary;
        }
    }
}
