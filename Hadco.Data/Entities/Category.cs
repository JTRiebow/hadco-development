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
    [Table("Categories")]
    public class Category : IModel
    {
        [Key]
        [Column("CategoryID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(128)]
        public string CategoryNumber { get; set; }

        [MaxLength(128)]
        public string JobNumber { get; set; }
        
        public int JobID { get; set; }

        [MaxLength(128)]
        public string PhaseNumber { get; set; }
        
        public int PhaseID { get; set; }
        public Phase Phase { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }   
        
        public string UnitsOfMeasure { get; set; }
        
        public decimal? PlannedQuantity { get; set; }
        public EntityStatus Status { get; set; }
    }
}
