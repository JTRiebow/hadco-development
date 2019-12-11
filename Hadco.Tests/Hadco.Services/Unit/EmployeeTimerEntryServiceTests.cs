using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using AutoMapper;
using Hadco.Common;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;

namespace Hadco.Tests.Hadco.Services.Unit
{
    [TestFixture()]
    public class EmployeeTimerEntryServiceTests
    {
        public EmployeeTimerEntryServiceTests()
        {
            MappingProfile.Configure();
        }
        [SetUp]

        [Test()]
        public void DeleteTest()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var currentUserMock = fixture.Create<Mock<ClaimsPrincipal>>();
            fixture.Inject<IPrincipal>(currentUserMock.Object);

            var employeeTimer = fixture.Create<EmployeeTimer>();
            var employeeTimerEntry = fixture.Create<EmployeeTimerEntry>();
            employeeTimerEntry.EmployeeTimer = employeeTimer;

            var id = employeeTimerEntry.ID;

            var employeeTimerEntryRepository = fixture.Freeze<Mock<IEmployeeTimerEntryRepository>>();
            employeeTimerEntryRepository.Setup(y => y.Find(id, x => x.EmployeeTimer)).Returns(employeeTimerEntry);
            var sut = fixture.Create<EmployeeTimerEntryService>();

            // Act
            sut.Delete(id);

            // Assert
            employeeTimerEntryRepository.Verify(x=>x.Delete(id), Times.Once);
        }

        [Test()]
        public void Insert_CallsInsertOnce_UserIsEmployee()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var dto = fixture.Create<EmployeeTimerEntryDto>();
            var employeeTimer = fixture.Build<EmployeeTimer>().With(x => x.ID, dto.EmployeeTimerID).Without(x=>x.EmployeeTimerEntries).Create();
            var employeeID = employeeTimer.EmployeeID;

            var employeeIDClaim = new Claim("employeeid", employeeID.ToString());
            var currentUserMock = fixture.Create<Mock<ClaimsPrincipal>>();
            currentUserMock.Setup(x => x.Claims).Returns(new List<Claim> { employeeIDClaim });
            fixture.Inject<IPrincipal>(currentUserMock.Object);

            var employeeTimerEntryRepository = fixture.Freeze<Mock<IEmployeeTimerEntryRepository>>();
            employeeTimerEntryRepository.Setup(x => x.All).Returns((IQueryable<EmployeeTimerEntry>)null);
            var employeeTimerRepository = fixture.Freeze<Mock<IEmployeeTimerRepository>>();
            employeeTimerRepository.Setup(x => x.Find(dto.EmployeeTimerID)).Returns(employeeTimer);
            var sut = fixture.Create<EmployeeTimerEntryService>();
            // Act
            sut.Insert(dto);

            // Assert
            employeeTimerEntryRepository.Verify(x => x.Insert(It.IsAny<EmployeeTimerEntry>()), Times.Once);
        }

        [Test()]
        public void UpdateTest_ChangeClockin()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var dto = fixture.Create<EmployeeTimerEntryDto>();
            var employeeTimer = fixture.Build<EmployeeTimer>().With(x => x.ID, dto.EmployeeTimerID).Create();
            var employeeID = employeeTimer.EmployeeID;

            var employeeIDClaim = new Claim("employeeid", employeeID.ToString());
            var currentUserMock = fixture.Create<Mock<ClaimsPrincipal>>();
            currentUserMock.Setup(x => x.Claims).Returns(new List<Claim> { employeeIDClaim });
            fixture.Inject<IPrincipal>(currentUserMock.Object);

            var employeeTimerEntryRepository = fixture.Freeze<Mock<IEmployeeTimerEntryRepository>>();

            var employeeTimerEntryOriginal = Mapper.Map<EmployeeTimerEntry>(dto);
            employeeTimerEntryOriginal.ClockIn = employeeTimerEntryOriginal.ClockIn.AddMinutes(-5);
            employeeTimerEntryRepository.Setup(x => x.Find(dto.ID)).Returns(employeeTimerEntryOriginal);

            var employeeTimerRepository = fixture.Freeze<Mock<IEmployeeTimerRepository>>();
            employeeTimerRepository.Setup(x => x.Find(dto.EmployeeTimerID)).Returns(employeeTimer);
            var sut = fixture.Create<EmployeeTimerEntryService>();

            // Act
            sut.Update(dto);

            // Assert
            employeeTimerEntryRepository.Verify(x => x.Save(), Times.Once);
        }

        [Test]
        public void Update_ChangeClockin_AddsHistoryEntry_ChangedClockIn()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var dto = fixture.Create<EmployeeTimerEntryDto>();
            dto.ClockOut = dto.ClockIn.AddMinutes(fixture.Create<double>());
            var employeeTimer = fixture.Build<EmployeeTimer>().With(x => x.ID, dto.EmployeeTimerID).Create();
            var employeeID = employeeTimer.EmployeeID;

            var employeeIDClaim = new Claim("employeeid", employeeID.ToString());
            var currentUserMock = fixture.Create<Mock<ClaimsPrincipal>>();
            currentUserMock.Setup(x => x.Claims).Returns(new List<Claim> { employeeIDClaim });
            fixture.Inject<IPrincipal>(currentUserMock.Object);

            var employeeTimerEntryRepository = fixture.Freeze<Mock<IEmployeeTimerEntryRepository>>();

            var employeeTimerEntryOriginal = Mapper.Map<EmployeeTimerEntry>(dto);
            employeeTimerEntryOriginal.ClockIn = employeeTimerEntryOriginal.ClockIn.AddMinutes(-5);
            employeeTimerEntryRepository.Setup(x => x.Find(dto.ID)).Returns(employeeTimerEntryOriginal);

            var employeeTimerRepository = fixture.Freeze<Mock<IEmployeeTimerRepository>>();
            employeeTimerRepository.Setup(x => x.Find(dto.EmployeeTimerID)).Returns(employeeTimer);

            var employeeTimerEntryHistoryRepository = fixture.Freeze<Mock<IEmployeeTimerEntryHistoryRepository>>();
            var sut = fixture.Create<EmployeeTimerEntryService>();

            // Act
            sut.Update(dto);

            // Assert
            employeeTimerEntryHistoryRepository.Verify(x => x.Insert(It.IsAny<EmployeeTimerEntryHistory>()), Times.Once);
        }

        [Test()]
        public void CheckForOverlappingEmployeeTimerEntriesTest()
        {
            // throw new NotImplementedException();
        }
    }
}