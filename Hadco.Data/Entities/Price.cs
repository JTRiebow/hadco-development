using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("Prices")]
    public class Price : IModel
    {
        [Key]
        [Column("PriceID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PricingID { get; set; }
        public Pricing Pricing { get; set; }
        public int? MaterialID { get; set; }
        public int? TruckClassificationID { get; set; }
        public decimal? Value { get; set; }
    }
}