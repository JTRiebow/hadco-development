using Hadco.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("Equipment")]
    public class Equipment : IModel
    {
        [Key]
        [Column("EquipmentID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(128)]
        public string EquipmentNumber { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(128)]
        public string Model { get; set; }

        [MaxLength(128)]
        public string License { get; set; }

        [MaxLength(128)]
        public string Fleetcode { get; set; }

        [MaxLength(128)]
        public string SerialNumber { get; set; }
        public int Mileage { get; set; }
        public decimal HoursOfOperation { get; set; }

        public string Memo { get; set; }
        public EntityStatus Status { get; set; }
        public string Type { get; set; }
    }
}
