using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;
using Hadco.Data.Entities;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Hadco.Tests.Hadco.Services.Builders
{
    public class PricesBuilder
    {
        public IEnumerable<Price> BuildMaterialPrices(int pricingID)
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());

            return fixture.Build<Price>()
                            .Without(x => x.TruckClassificationID)
                            .CreateMany();
        }

        public IEnumerable<Price> BuildTruckPrices(int pricingID)
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());

            return fixture.Build<Price>()
                            .Without(x => x.MaterialID)
                            .CreateMany();
        }
    }

    public class TruckPrice
    {
        [Key]
        [Column("PriceID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PricingID { get; set; }
        public int TruckClassificationID { get; set; }
        public decimal? Value { get; set; }
    }

    public class MaterialPrice
    {
        [Key]
        [Column("PriceID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PricingID { get; set; }
        public int MaterialID { get; set; }
        public decimal? Value { get; set; }
    }
}
