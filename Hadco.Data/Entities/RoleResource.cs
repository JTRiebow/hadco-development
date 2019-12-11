using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
	public class RoleResource
	{
		[Key, Column(Order = 0)]
		public int RoleID { get; set; }
		public virtual Role Role { get; set; }

		[Key, Column(Order = 1)]
		public int ResourceID { get; set; }
		public virtual Resource Resource { get; set; }

		public bool Read { get; set; }
		public bool Write { get; set; }
		public bool Delete { get; set; }
	}
}
