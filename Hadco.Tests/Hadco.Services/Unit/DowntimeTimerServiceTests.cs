using System;
using System.Security.Claims;
using Hadco.Data;
using Ploeh.AutoFixture;
using Hadco.Data.Entities;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Hadco.Tests.Hadco.Services.Builders;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture.AutoMoq;
using System.Security.Principal;
using System.Collections.Generic;

namespace Hadco.Tests.Hadco.Services.Unit
{
    [TestFixture()]
    public class DowntimeTimerServiceTests
    {
        public DowntimeTimerServiceTests()
        {
            // TODO Refactor to allow for DI of mapping configuration. AutoMapper's static API is becoming obsolete too https://lostechies.com/jimmybogard/2016/01/21/removing-the-static-api-from-automapper/
            MappingProfile.Configure();
        }

        [Test]
        public void Insert_AddsTimesheetIDIfMissing()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            fixture.Inject<IPrincipal>(fixture.Create<Mock<ClaimsPrincipal>>().Object);

            var loadTimer = fixture.Create<LoadTimer>();
            var loadTimerID = loadTimer.ID;
            var dto = fixture.Build<DowntimeTimerDto>().With(x => x.LoadTimerID, loadTimerID).Without(x => x.TimesheetID).Create();
          
            var loadTimerRepository = fixture.Freeze<Mock<ILoadTimerRepository>>();
            var downtimeTimerRepository = fixture.Freeze<Mock<IDowntimeTimerRepository>>();
            loadTimerRepository.Setup(x => x.Find(loadTimerID)).Returns(loadTimer);
            var sut = fixture.Create<DowntimeTimerService>();

            // Act
            sut.Insert(dto);

            // Assert
            downtimeTimerRepository.Verify(x => x.Insert(It.Is<DowntimeTimer>(y => y.TimesheetID == loadTimer.TimesheetID)), Times.Once);
        }

        [Test]
        public void Insert_AddsSystemProducedDowntime()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            fixture.Inject<IPrincipal>(fixture.Create<Mock<ClaimsPrincipal>>().Object);

            var dto = fixture.Create<DowntimeTimerDto>();
            var downtimeTimerRepository = fixture.Freeze<Mock<IDowntimeTimerRepository>>();
            var sut = fixture.Create<DowntimeTimerService>();

            // Act
            sut.Insert(dto);

            // Assert
            downtimeTimerRepository.Verify(x => x.AddLoadDowntimeTimers(dto.TimesheetID.Value), Times.Once);
        }

        [Test]
        public void Update_AddsTimesheetIDIfMissing()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            fixture.Inject<IPrincipal>(fixture.Create<Mock<ClaimsPrincipal>>().Object);

            var loadTimer = fixture.Create<LoadTimer>();
            var loadTimerID = loadTimer.ID;
            var dto = fixture.Create<DowntimeTimerDto>();
            dto.TimesheetID = null;
            dto.LoadTimerID = loadTimerID;

            var loadTimerRepository = fixture.Freeze<Mock<ILoadTimerRepository>>();
            var downtimeTimerRepository = fixture.Freeze<Mock<IDowntimeTimerRepository>>();
            loadTimerRepository.Setup(x => x.Find(loadTimerID)).Returns(loadTimer);

            var sut = fixture.Create<DowntimeTimerService>();

            // Act
            sut.Update(dto);

            // Assert
            Assert.AreEqual(dto.TimesheetID, loadTimer.TimesheetID);
            downtimeTimerRepository.Verify(x => x.Save(), Times.Once);
        }

        [Test]
        public void Update_AddsSystemProducedDowntime()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            fixture.Inject<IPrincipal>(fixture.Create<Mock<ClaimsPrincipal>>().Object);

            var dto = fixture.Create<DowntimeTimerDto>();
            var downtimeTimerRepository = fixture.Freeze<Mock<IDowntimeTimerRepository>>();
            var sut = fixture.Create<DowntimeTimerService>();

            // Act
            sut.Update(dto);

            // Assert
            downtimeTimerRepository.Verify(x => x.AddLoadDowntimeTimers(dto.TimesheetID.Value), Times.Once);
        }
    }
}