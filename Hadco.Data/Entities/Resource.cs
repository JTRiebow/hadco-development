using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Hadco.Data.Entities
{
	public class Resource : IModel
	{
		[Column("ResourceID")]
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

		[Required]
		[MaxLength(128)]
        [Index(IsUnique=true)]
		public string Name { get; set; }

		public ICollection<RoleResource> RoleResources { get; set; }
	}
}
