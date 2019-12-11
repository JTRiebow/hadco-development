using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Geocoding.Yahoo;
using Hadco.Common;
using Hadco.Common.Enums;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Hadco.Tests.Hadco.Services.Unit
{
    [TestFixture]
    public class EmployeeTimerServiceTests
    {
        private Fixture fixture = new Fixture();
        private int currentUserID;
        private Mock<ClaimsPrincipal> currentUser;
        private Mock<IDailyApprovalService> dailyApprovalService;

        [Test]
        public void Insert_CreatesDailyApprovalIfDoesntExists()
        {
            var dto = fixture.Create<EmployeeTimerDto>();

            var day = dto.Day;
            var employeeID = dto.EmployeeID;
            var departmentID = dto.DepartmentID;

            dailyApprovalService = fixture.Freeze<Mock<IDailyApprovalService>>();
            dailyApprovalService.Setup(x => x.Get(employeeID, day, departmentID)).Returns<DailyApprovalDto>(null);
            fixture.Inject(dailyApprovalService.Object);

            var sut = fixture.Create<EmployeeTimerService>();
            sut.Insert(dto);

            dailyApprovalService.Verify(x => x.Insert(It.Is<DailyApprovalDto>(
                y => y.EmployeeID == employeeID && y.Day == day && y.DepartmentID == departmentID)), Times.Once);
        }

        [Test]
        public void Insert_DoesNotCreateDailyApprovalIfAlreadyExists()
        {
            var dto = fixture.Create<EmployeeTimerDto>();

            var day = dto.Day;
            var employeeID = dto.EmployeeID;
            var departmentID = dto.DepartmentID;

            dailyApprovalService = fixture.Freeze<Mock<IDailyApprovalService>>();

            fixture.Inject(dailyApprovalService.Object);

            var sut = fixture.Create<EmployeeTimerService>();
            sut.Insert(dto);

            dailyApprovalService.Verify(x
                => x.Insert(It.Is<DailyApprovalDto>(y
                    => y.EmployeeID == employeeID && y.Day == day && y.DepartmentID == departmentID)
                ), Times.Never);
        }

        [Test]
        [TestCase(DepartmentName.Concrete)]
        [TestCase(DepartmentName.Concrete2H)]
        [TestCase(DepartmentName.ConcreteHB)]
        [TestCase(DepartmentName.Development)]
        [TestCase(DepartmentName.Residential)]
        [TestCase(DepartmentName.Mechanic)]
        [TestCase(DepartmentName.Trucking)]
        [TestCase(DepartmentName.Transport)]
        [TestCase(DepartmentName.TMCrushing)]
        public void Insert_CreatesTimesheetIfDepartmentMismatch(DepartmentName departmentID)
        {
            // Employee has timesheet with different department
            var dto = fixture.Create<EmployeeTimerDto>();
            dto.DepartmentID = (int)departmentID;

            var timesheetRepository = fixture.Create<Mock<ITimesheetService>>();
            var timesheetDto = fixture.Build<TimesheetDto>().With(x => x.EmployeeID, dto.EmployeeID).Create();
            timesheetRepository.Setup(x => x.Find(dto.TimesheetID.Value, true)).Returns(timesheetDto);
            fixture.Inject(timesheetRepository.Object);
            fixture.Freeze<TimesheetService>();

            var sut = fixture.Create<EmployeeTimerService>();
            sut.Insert(dto);
            timesheetRepository.Verify(x
                                           => x.GetOrCreateTimesheet(It.Is<int>(y => y == dto.EmployeeID), It.Is<DateTime>(y => y == dto.Day), It.Is<int>(y => y == dto.DepartmentID)), Times.Once);
        }

        [Test]
        [TestCase(DepartmentName.Concrete)]
        [TestCase(DepartmentName.Concrete2H)]
        [TestCase(DepartmentName.ConcreteHB)]
        [TestCase(DepartmentName.Development)]
        [TestCase(DepartmentName.Residential)]
        [TestCase(DepartmentName.Mechanic)]
        [TestCase(DepartmentName.Trucking)]
        [TestCase(DepartmentName.Transport)]
        [TestCase(DepartmentName.TMCrushing)]
        public void Insert_CreatesTimesheetIfDoesntExists(DepartmentName departmentID)
        {
            var dto = fixture.Create<EmployeeTimerDto>();
            dto.DepartmentID = (int)departmentID;

            var timesheetRepository = fixture.Create<Mock<ITimesheetService>>();
            timesheetRepository.Setup(x => x.Find(dto.TimesheetID.Value, true)).Returns((TimesheetDto)null);
            fixture.Inject(timesheetRepository.Object);
            fixture.Freeze<TimesheetService>();

            var sut = fixture.Create<EmployeeTimerService>();
            sut.Insert(dto);
            timesheetRepository.Verify(x
                => x.GetOrCreateTimesheet(It.Is<int>(y => y == dto.EmployeeID), It.Is<DateTime>(y => y == dto.Day), It.Is<int>(y => y == dto.DepartmentID)), Times.Once);
        }

        [SetUp]
        public void Setup()
        {
            MappingProfile.Configure();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            currentUserID = fixture.Create<int>();
            SetupAuthorizedUser();
        }

        private void SetupAuthorizedUser()
        {
            currentUser = fixture.Create<Mock<ClaimsPrincipal>>();
            fixture.Inject<IPrincipal>(currentUser.Object);
            var claims = new List<Claim>()
                         {
                             new Claim(ClaimsExtensions.EMPLOYEEID_KEY, currentUserID.ToString())
                         };
            currentUser.Setup(x => x.Claims)
                       .Returns(claims);
        }
    }
}
