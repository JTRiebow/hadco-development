using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("Roles")]
    public class Role : IModel
    {
        [Key]
        [Column("RoleID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(128)]
        [Required]
        [Index(IsUnique=true)]
        public string Name { get; set; }
    }
}
