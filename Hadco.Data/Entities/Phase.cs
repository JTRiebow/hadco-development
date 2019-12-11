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
    [Table("Phases")]
    public class Phase : IModel
    {
        [Key]
        [Column("PhaseID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(128)]
        public string PhaseNumber { get; set; }
        
        public Job Job { get; set; }
        public int JobID { get; set; }

        [MaxLength(128)]
        public string JobNumber { get; set; }
   
        [MaxLength(128)]
        public string Name { get; set; }
        public EntityStatus Status { get; set; }
    }
}
