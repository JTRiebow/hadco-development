using System.Security.Claims;
using AutoMapper;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Moq;

namespace Hadco.Tests.Hadco.Services.Builders
{
    public class JobTimerServiceBuilder
    {
        public Mock<IJobTimerRepository> mockJobTimerRepository;
        public Mock<ITimesheetRepository> mockTimesheetRepository;
        public Mock<ClaimsPrincipal> mockcurrentUser;

        public JobTimerServiceBuilder()
        {
            mockJobTimerRepository = new Mock<IJobTimerRepository>();
            mockTimesheetRepository = new Mock<ITimesheetRepository>();
            mockcurrentUser = new Mock<ClaimsPrincipal>();
        }

    }
}
