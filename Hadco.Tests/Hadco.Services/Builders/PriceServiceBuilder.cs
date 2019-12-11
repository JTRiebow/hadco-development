using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services;
using Moq;

namespace Hadco.Tests.Hadco.Services.Builders
{
    public class PriceServiceBuilder
    {
        public Mock<IPricingRepository> mockPricingRepository;
        public Mock<IPriceRepository> mockPriceRepository;
        public Mock<ClaimsPrincipal> mockcurrentUser;

        public PriceServiceBuilder()
        {
            mockPricingRepository = new Mock<IPricingRepository>();
            mockPriceRepository = new Mock<IPriceRepository>();
            mockcurrentUser = new Mock<ClaimsPrincipal>();
        }

        //public PriceService Build()
        //{
        //    return new PriceService(mockPriceRepository.Object, mockPricingRepository.Object, mockcurrentUser.Object);
        //}
    }
}
