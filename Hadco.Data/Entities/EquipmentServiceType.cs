using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("EquipmentServiceTypes")]
    public class EquipmentServiceType : IModel
    {
        [Key]
        [Column("EquipmentServiceTypeID")]
        public int ID { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }

        [MaxLength(4)]
        public string Code { get; set; }
    }
}