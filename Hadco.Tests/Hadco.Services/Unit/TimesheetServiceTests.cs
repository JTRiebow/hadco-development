using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Hadco.Tests.Hadco.Services.Unit
{
    [TestFixture()]
    public class TimesheetServiceTests
    {
        [Test]
        public void FindExpandedExpectedPath()
        {
            MappingProfile.Configure();
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var currentUserID = fixture.Create<int>();
            TestHelpers.SetupAuthorizedUser(fixture, currentUserID);

            var timesheet = fixture.Create<Timesheet>();
            var timesheetRepository = fixture.Freeze<Mock<ITimesheetRepository>>();
            timesheetRepository.Setup(x => x.AllIncluding(It.IsAny<Expression<Func<Timesheet, object>>[]>())).Returns((() => new List<Timesheet>(){timesheet}.AsQueryable()));

            var sut = fixture.Create<TimesheetService>();
            var result = sut.FindExpanded(timesheet.EmployeeID, timesheet.Day, timesheet.DepartmentID, true);
            Assert.That(result != null);
        }

        [Test]
        [Category(TestConstants.Bug)]
        public void EquipmentTimerIDMaps()
        {
            MappingProfile.Configure();
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var currentUserID = fixture.Create<int>();
            TestHelpers.SetupAuthorizedUser(fixture, currentUserID);

            var timesheet = fixture.Create<Timesheet>();
            timesheet.Day = timesheet.Day.Date;
            var timesheetRepository = fixture.Freeze<Mock<ITimesheetRepository>>();
            timesheetRepository.Setup(x => x.AllIncluding(It.IsAny<Expression<Func<Timesheet, object>>[]>())).Returns((() => new List<Timesheet>() { timesheet }.AsQueryable()));

            var sut = fixture.Create<TimesheetService>();
            var result = sut.FindExpanded(timesheet.EmployeeID, timesheet.Day, timesheet.DepartmentID, true);

            Assert.That(result.EquipmentTimers.Select(x => x.EquipmentTimerID).SequenceEqual(timesheet.EquipmentTimers.Select(y => y.ID)));
            Assert.That(result.EquipmentTimers.Select(x => x.ID).SequenceEqual(timesheet.EquipmentTimers.Select(y => y.EquipmentTimerEntryID.Value)));
            // Assert.That(result.EquipmentTimers.All(x => x.EquipmentTimerID != 0));
        }

        [Test]
        [Category(TestConstants.Bug)]
        public void EquipmentTimerIDsMap()
        {
            MappingProfile.Configure();
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var currentUserID = fixture.Create<int>();
            TestHelpers.SetupAuthorizedUser(fixture, currentUserID);

            var timesheet = fixture.Create<Timesheet>();
            timesheet.Day = timesheet.Day.Date;
            var timesheetRepository = fixture.Freeze<Mock<ITimesheetRepository>>();
            timesheetRepository.Setup(x => x.AllIncluding(It.IsAny<Expression<Func<Timesheet, object>>[]>())).Returns((() => new List<Timesheet>() { timesheet }.AsQueryable()));

            var sut = fixture.Create<TimesheetService>();
            var result = sut.FindExpanded(timesheet.EmployeeID, timesheet.Day, timesheet.DepartmentID, true);

            Assert.That(result.EquipmentTimers.Select(x => x.TimesheetID).SequenceEqual(timesheet.EquipmentTimers.Select(y => y.TimesheetID)));
            Assert.That(result.EquipmentTimers.Select(x=>x.EquipmentID).SequenceEqual(timesheet.EquipmentTimers.Select(y=>y.EquipmentID)));
            Assert.That(result.EquipmentTimers.Select(x => x.EquipmentServiceTypeID).SequenceEqual(timesheet.EquipmentTimers.Select(y => y.EquipmentServiceTypeID.Value)));
        }

    }
}
