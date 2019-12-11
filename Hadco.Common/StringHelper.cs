using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Common
{
	public class StringHelper
	{
		//Removes all non numeric characters from the string
		public static string GetNumericString(string input)
		{
			return new string(input.Where(c => char.IsDigit(c)).ToArray());
		}

		public static string GetRandomAlphanumericString()
		{
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var random = new Random();
			var result = new string(
				Enumerable.Repeat(chars, 8)
							.Select(s => s[random.Next(s.Length)])
							.ToArray());
			return result;
		}
	}
}
