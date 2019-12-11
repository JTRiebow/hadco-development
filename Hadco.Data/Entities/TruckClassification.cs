using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("TruckClassifications")]
    public class TruckClassification : IModel
    {
        [Key]
        [Column("TruckClassificationID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [MaxLength(32)]
        [Required]
        public string Name { get; set; }

        [MaxLength(16)]
        [Required]
        public string Truck { get; set; }

        [MaxLength(16)]
        public string Trailer1 { get; set; }

        [MaxLength(16)]
        public string Trailer2 { get; set; }

        [MaxLength(16)]
        public string Code{ get; set; }
    }
}