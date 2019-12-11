using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System.Data.SqlClient;
using Hadco.Common.Enums;

namespace Hadco.Tests.Hadco.Services.Builders
{
    public class PricingsBuilder
    {
        public PricingsBuilder()
        {
        }

        public IEnumerable<Pricing> BuildDevelopmentalHourlyPricings()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
           var pricings = fixture.Build<Pricing>()
                .With(x=>x.ID)
                .With(x => x.CustomerTypeID, (int) CustomerTypeName.Development)
                .With(x => x.BillTypeID, (int) BillTypeName.Hourly)
                .With(x => x.JobID)
                .OmitAutoProperties()
                .CreateMany();

            foreach (Pricing pricing in pricings)
            {
                
                pricing.Prices = BuildTruckPrices(pricing.ID).ToList();
            }

            return pricings;
        }
        public IEnumerable<Pricing> BuildDevelopmentalLoadPricings()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            var pricings = fixture.Build<Pricing>()
                                            .With(x => x.CustomerTypeID, (int)CustomerTypeName.Development)
                                            .With(x => x.BillTypeID, (int)BillTypeName.Load)
                                            .With(x=>x.JobID)
                                            .OmitAutoProperties()
                                            .CreateMany();
            foreach (var pricing in pricings)
            {

                pricing.Prices = BuildMaterialPrices(pricing.ID).ToList();
            }

            return pricings;
        }
        public IEnumerable<Price> BuildMaterialPrices(int pricingID)
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());

            return fixture.Build<Price>()
                            .With(x=>x.PricingID, pricingID)
                            .Without(x => x.TruckClassificationID)
                            .CreateMany();
        }

        public IEnumerable<Price> BuildTruckPrices(int pricingID)
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());

            return fixture.Build<Price>()
                            .With(x => x.PricingID, pricingID)
                            .Without(x => x.MaterialID)
                            .CreateMany();
        }

        public void Build()
        {
            var test = new PricingsBuilder();

            
            
        }
    }

    //public class JobTruckPricing
    //{
    //    [Column("PricingID")]
    //    [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int ID { get; set; }

    //    public int CustomerTypeID
    //    {
    //        set { value = (int)CustomerType.Developmental; }
    //    }

    //    public int BillTypeID
    //    {
    //        set { value = (int)BillTypeName.Hourly;}
    //    }

    //    public int JobID { get; set; }

    //    public DateTimeOffset StartTime { get; set; }
    //    public DateTimeOffset? EndTime { get; set; }

    //    public virtual ICollection<Price> Prices { get; set; }
    //}

    //public class JobMaterialPricing
    //{
    //    [Column("PricingID")]
    //    [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int ID { get; set; }
    //    public int CustomerTypeID { get; set; }
    //    public int BillTypeID { get; set; }

    //    public int JobID { get; set; }

    //    public DateTimeOffset StartTime { get; set; }
    //    public DateTimeOffset? EndTime { get; set; }

    //    public virtual ICollection<Price> Prices { get; set; }
    //}

    //public class CustomerTruckPricing
    //{
    //    [Column("PricingID")]
    //    [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int ID { get; set; }
    //    public int CustomerTypeID { get; set; }
    //    public int BillTypeID { get; set; }

    //    public int CustomerID { get; set; }

    //    public DateTimeOffset StartTime { get; set; }
    //    public DateTimeOffset? EndTime { get; set; }

    //    public virtual ICollection<Price> Prices { get; set; }
    //}

    //public class CustomerMaterialPricing
    //{
    //    [Column("PricingID")]
    //    [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int ID { get; set; }
    //    public int CustomerTypeID { get; set; }
    //    public int BillTypeID { get; set; }

    //    public int CustomerID { get; set; }

    //    public DateTimeOffset StartTime { get; set; }
    //    public DateTimeOffset? EndTime { get; set; }

    //    public virtual ICollection<Price> Prices { get; set; }
    //}

    //public class RunMaterialPricing
    //{
    //    [Column("PricingID")]
    //    [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int ID { get; set; }
    //    public int CustomerTypeID { get; set; }
    //    public int BillTypeID { get; set; }

    //    public int RunID { get; set; }

    //    public DateTimeOffset StartTime { get; set; }
    //    public DateTimeOffset? EndTime { get; set; }

    //    public virtual ICollection<Price> Prices { get; set; }
    //}


}
