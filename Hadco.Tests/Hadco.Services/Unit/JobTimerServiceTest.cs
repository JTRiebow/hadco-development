using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Hadco.Data.Entities;
using Hadco.Services.DataTransferObjects;
using Hadco.Tests.Hadco.Services.Builders;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using AutoMapper;
using Dapper;
using Ploeh.AutoFixture.AutoMoq;
using Hadco.Services;
using Hadco.Data;
using System.Security.Principal;
using System.Security.Claims;

namespace Hadco.Tests.Hadco.Services.Unit
{
    [TestFixture]
    public class JobTimerServiceTest
    {
        public JobTimerServiceTest()
        {
            MappingProfile.Configure();
        }

        [Test]
        public void Insert_NoOverlap_JobTimerDto()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var currentUserMock = fixture.Create<Mock<ClaimsPrincipal>>();
            fixture.Inject<IPrincipal>(currentUserMock.Object);

            var timesheet = fixture.Build<Timesheet>().With(x => x.ID).With(x => x.EmployeeID).OmitAutoProperties().Create();
            var timesheetID = timesheet.ID;

            var sut = fixture.Create<JobTimerService>();
            var jobTimers = CreateJobTimers(timesheet);

            var timesheetRepositoryMock = fixture.Create<Mock<TimesheetRepository>>();
            timesheetRepositoryMock.Setup(x => x.Find(timesheetID)).Returns(timesheet);

            var jobTimerRepositoryMock = fixture.Create<Mock<JobTimerRepository>>();

            var jobTimerDto = Mapper.Map<JobTimerDto>(jobTimers.FirstOrDefault());

            // Act
            //sut.Insert(jobTimerDto);

            // Assert

        }

        public List<JobTimer> CreateJobTimers(Timesheet timesheet)
        {
            var timesheetID = timesheet.ID;

            // this datetime is just used for testing. 
            var midnight = new DateTimeOffset(2016, 02, 04, 0, 0, 0, new TimeSpan(-7, 0, 0));

            var jobOne2Two = new JobTimer
            {
                ID = 1,
                StartTime = midnight.AddHours(1),
                StopTime = midnight.AddHours(2),
                TimesheetID = timesheetID
            };
            var jobTwo2Three = new JobTimer
            {
                ID = 2,
                StartTime = midnight.AddHours(2),
                StopTime = midnight.AddHours(3),
                TimesheetID = timesheetID
            };
            var jobFive2Seven = new JobTimer
            {
                ID = 3,
                StartTime = midnight.AddHours(5),
                StopTime = midnight.AddHours(7),
                TimesheetID = timesheetID
            };

            var jobList = new List<JobTimer> { jobOne2Two, jobTwo2Three, jobFive2Seven };
            return jobList;
        }

        public List<JobTimerDto> CreateJobTimerDtos(Timesheet timesheet)
        {
            var timesheetID = timesheet.ID;

            var now = DateTimeOffset.Now;

            var jobNow = new JobTimerDto
            {
                ID = 1,
                StartTime = now.AddHours(-1),
                StopTime = now.AddHours(1),
                TimesheetID = timesheetID
            };
            var jobOld = new JobTimerDto
            {
                ID = 2,
                StartTime = now.AddHours(-10),
                StopTime = now.AddHours(-9),
                TimesheetID = timesheetID
            };
            var jobConflict = new JobTimerDto
            {
                ID = 3,
                StartTime = now.AddHours(-9.5),
                StopTime = now,
                TimesheetID = timesheetID
            };

            var jobList = new List<JobTimerDto> { jobNow, jobOld, jobConflict };
            return jobList;
        }
    }
}
