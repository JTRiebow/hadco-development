using Hadco.Data.Entities;
using Hadco.Common;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using Moq;
using System.Data.SqlClient;
using System.Runtime.Remoting.Contexts;
using AutoMapper.Internal;
using Hadco.Data;
using Hadco.Data.Migrations;
using Hadco.Tests.DataPopulator;
using Hadco.Tests.Hadco.Services.Builders;
using NUnit.Framework;
using NUnit.VisualStudio.TestAdapter;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Hadco.Tests.Hadco.Services.Unit
{
    [TestFixture]
    public class PriceServiceTests
    {
        [TestFixtureSetUp]
        public void CreateDatabaseWithPriceData()
        {
            var test = new PricingData();
            test.SetPricingData();
        }

        [Test]
        public void GetPricesByIDTest()
        {
            // Arrange    
            var pricingsBuilder = new PricingsBuilder();
            var pricings = pricingsBuilder.BuildDevelopmentalHourlyPricings().AsQueryable();
            var pricingID = pricings.FirstOrDefault().ID;
                                    
            var builder = new PriceServiceBuilder();
            var priceService = builder.Build();

            builder.mockPricingRepository.Setup(x => x.GetPricesByID(pricingID))
                    .Returns(pricings.Where(x=>x.ID == pricingID));

            int resultCount;
            int totalResultCount;

            // Act
            var result = priceService.GetPricesByID(pricingID, out resultCount, out totalResultCount);

            // Assert

            Assert.IsNotNull(result);
            builder.mockPricingRepository.Verify(x => x.GetPricesByID(pricingID), Times.Once);
        }


        //[Test]
        //[TestCase(1,(int)BillTypeEnum.Hourly,1,1,1, "Trucks")]
        //public void GetPriceHistoryTest(int customerTypeID, int billTypeID, int? jobID, int? customerID, int? runID)
        //{

        //    // Arrange

        //    var builder = new PriceServiceBuilder();
        //    var priceService = builder.Build();

        //    builder.mockPricingRepository.Setup(x => x.GetHistoryTrucks(customerTypeID,
        //                                                        (int)BillTypeEnum.Hourly,
        //                                                        jobID,
        //                                                        customerID,
        //                                                        runID));
        //    // Act

        //    priceService.GetPriceHistory(customerTypeID, billTypeID, jobID, customerTypeID, runID);

        //    // Assert

        //}
    }
}
