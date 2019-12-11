using Hadco.Common;
using Hadco.Common.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("Pricings")]
    public class Pricing : IModel
    {
        [Column("PricingID")]
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int CustomerTypeID { get; set; }
        public CustomerType CustomerType { get; set; }

        public int BillTypeID { get; set; }
        public virtual BillType BillType { get; set; }

        public int? PhaseID { get; set; }
        public virtual Phase Phase { get; set; }

        public int? JobID { get; set; }
        public virtual Job Job { get; set; }

        public int? CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Column(TypeName = "Date")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? EndDate { get; set; }

        public DateTimeOffset UpdatedTime { get; set; }

        public virtual ICollection<Price> Prices { get; set; }
    }
}