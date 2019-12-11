using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("Materials")]    
    public class Material : IModel 
    {
        [Key]
        [Column("MaterialID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }
        
        [MaxLength(128)]
        public string CategoryNumber { get; set; }
    }
}
