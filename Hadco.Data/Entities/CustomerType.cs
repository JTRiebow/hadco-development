using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("CustomerTypes")]
    public class CustomerType : IModel
    {
        [Key]
        [Column("CustomerTypeID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }
    }
}