using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Moq;

namespace Hadco.Tests.Hadco.Services.Builders
{
    public class LoadTimerServiceBuilder
    {
        public Mock<ILoadTimerRepository> mockLoadTimerRepository;
        public Mock<ILoadTimerEntryRepository> mockLoadTimerEntryRepository;
        public Mock<IDowntimeTimerRepository> mockDowntimeTimerRepository;
        public Mock<ICategoryRepository> mockCategoryRepository;
        public Mock<IPhaseRepository> mockPhaseRepository;
        public Mock<ITruckClassificationRepository> mockTruckClassificationRepository;
        public Mock<ILoadTimerEntryService> mockLoadTimerEntryService;
        public Mock<IPricingService> mockPricingService;
        public Mock<ClaimsPrincipal> mockcurrentUser;

        public LoadTimerServiceBuilder()
        {
            mockLoadTimerRepository= new Mock<ILoadTimerRepository>();
            mockLoadTimerEntryRepository = new Mock<ILoadTimerEntryRepository>();
            mockDowntimeTimerRepository = new Mock<IDowntimeTimerRepository>();
            mockCategoryRepository = new Mock<ICategoryRepository>();
            mockPhaseRepository = new Mock<IPhaseRepository>();
            mockTruckClassificationRepository = new Mock<ITruckClassificationRepository>();
            mockLoadTimerEntryService = new Mock<ILoadTimerEntryService>();
            mockPricingService = new Mock<IPricingService>();
            mockcurrentUser = new Mock<ClaimsPrincipal>();
        }

        public LoadTimerService Build()
        {
            Mapper.CreateMap<LoadTimer, LoadTimerDto>().ReverseMap();
            throw new NotImplementedException();
            //return new LoadTimerService(mockLoadTimerRepository.Object, 
            //                        mockDowntimeTimerRepository.Object,
            //                        mockCategoryRepository.Object, 
            //                        mockPhaseRepository.Object,
            //                        mockTruckClassificationRepository.Object,
            //                        mockPricingService.Object,
            //                        mockcurrentUser.Object);
        }
    }
}
