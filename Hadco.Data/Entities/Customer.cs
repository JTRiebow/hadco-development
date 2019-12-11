using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("Customers")]
    public class Customer : IModel
    {
        [Key]
        [Column("CustomerID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(128)]
        public string CustomerNumber { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }
    }
}
