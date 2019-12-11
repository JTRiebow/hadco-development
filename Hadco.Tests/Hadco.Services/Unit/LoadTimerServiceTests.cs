using System;
using System.Collections.Generic;
using System.Linq;
using Hadco.Common.Enums;
using Hadco.Data.Entities;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Hadco.Tests.Hadco.Services.Builders;
using Moq;
using NUnit.Framework;

namespace Hadco.Tests.Hadco.Services.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class LoadTimerServiceTest
    {
        private static DateTimeOffset? loadTime = DateTimeOffset.Now.AddHours(-4);
        private static  DateTimeOffset? dumpTime = DateTimeOffset.Now;
        private LoadTimerDto loadTimerDto;
        private List<Category> categories;
        private List<Phase> phases;   

        [TestFixtureSetUp]

        public void Setup()
        {
            MappingProfile.Configure();

            categories = CreateCategories();
            phases = CreatePhases();
        }

        [Test]
        [TestCase(1, 1, 1)]
        [Ignore("Needs to be updated")]
        public void LoadTimerService_Insert(int jobID, int phaseID, int categoryID)
        {
            // Arrange

            loadTimerDto = new LoadTimerDto
            {
                JobID = jobID,
                PhaseID = phaseID,
                CategoryID = categoryID
            };

            var builder = new LoadTimerServiceBuilder();
            var loadTimerService = builder.Build();

            builder.mockCategoryRepository.Setup(x => x.Find(categoryID))
                                        .Returns(categories.AsQueryable().FirstOrDefault(x => x.ID == categoryID));
            builder.mockPhaseRepository.Setup(x => x.Find(phaseID))
                                        .Returns(phases.AsQueryable().FirstOrDefault(x => x.ID == phaseID));
            builder.mockLoadTimerRepository.Setup(x => x.Insert(It.IsAny<LoadTimer>())).Returns((LoadTimer lt) => lt);

            // Act

            loadTimerService.Insert(loadTimerDto);

            // Assert

            builder.mockLoadTimerRepository.Verify(x => x.Insert(It.IsAny<LoadTimer>()), Times.Once);
        }

        [Test]
        [Ignore("Needs to be updated")]
        public void LoadTimerService_InsertOldTimer()
        {
            // Arrange
            Setup();
            var builder = new LoadTimerServiceBuilder();
            var loadTimerService = builder.Build();

            var timesheet = new Timesheet()
            {
                ID = 1,
                DepartmentID = (int)DepartmentName.Trucking,
                EmployeeID = 1,
                Day = DateTime.Now
            };

            var loadTimer = new LoadTimerDto()
            {
                TimesheetID = 1,
                LoadTime = loadTime,
                LoadTimeLatitude = (decimal?)1.1,
                LoadTimeLongitude = (decimal?)2.2,
                DumpTimeLatitude = (decimal?)3.3,
                DumpTimeLongitude = (decimal?)4.4,
                DumpTime = dumpTime
            };

           // builder.mockTimesheetRepository.Setup(x => x.Find(1)).Returns(timesheet);

            // Act
            loadTimerService.Insert(loadTimer); ;

            // Assert
            builder.mockLoadTimerRepository.Verify(x => x.Insert(It.IsAny<LoadTimer>()), Times.Once);
            builder.mockLoadTimerRepository.Verify(x => x.Insert(It.Is<LoadTimer>(y => y.LoadTimerEntries.SingleOrDefault(z => z.StartTime == loadTime && z.EndTime == dumpTime) != null)), Times.Once);
        }

        [Test]
        [Ignore("Needs to be updated")]
        public void LoadTimerService_InsertTimerWithEntries()
        {
            // Arrange
            MappingProfile.Configure();

            var builder = new LoadTimerServiceBuilder();
            var loadTimerService = builder.Build();

            var timesheet = new Timesheet()
            {
                ID = 1,
                DepartmentID = (int)DepartmentName.Trucking,
                EmployeeID = 1,
                Day = DateTime.Now
            };

            var loadTimer = new LoadTimerDto()
            {
                TimesheetID = 1,
                LoadTimerEntries = new List<LoadTimerEntryDto>()
                {
                    new LoadTimerEntryDto()
                    {
                        StartTime = loadTime,
                        StartTimeLatitude = (decimal?)1.1,
                        StartTimeLongitude = (decimal?)2.2,
                        EndTimeLatitude = (decimal?)3.3,
                        EndTimeLongitude = (decimal?)4.4,
                        EndTime = dumpTime
                    }
                }
            };

           // builder.mockTimesheetRepository.Setup(x => x.Find(1)).Returns(timesheet);

            // Act
            loadTimerService.Insert(loadTimer);

            // Assert
            builder.mockLoadTimerRepository.Verify(x => x.Insert(It.IsAny<LoadTimer>()), Times.Once);
        }

        [Test]
        [Ignore("Needs to be updated")]
        public void LoadTimerService_InsertOldTimerWithMissingData()
        {
            // Arrange
            MappingProfile.Configure();

            var builder = new LoadTimerServiceBuilder();
            var loadTimerService = builder.Build();

            var timesheet = new Timesheet()
            {
                ID = 1,
                DepartmentID = (int)DepartmentName.Trucking,
                EmployeeID = 1,
                Day = DateTime.Now
            };

            var loadTimer = new LoadTimerDto()
            {
                TimesheetID = 1,
                LoadTime = loadTime
            };

           // builder.mockTimesheetRepository.Setup(x => x.Find(1)).Returns(timesheet);

            // Act
            loadTimerService.Insert(loadTimer);

            // Assert
            builder.mockLoadTimerRepository.Verify(x => x.Insert(It.IsAny<LoadTimer>()), Times.Once);
            builder.mockLoadTimerRepository
                .Verify(x => x.Insert(It.Is<LoadTimer>(y => y.LoadTimerEntries.SingleOrDefault(z => z.StartTime == loadTime && z.EndTime == null) != null)), Times.Once);
        }

        [Test]
        [TestCase(1, 2, 1)]
        [TestCase(1, 1, 2)]
        [TestCase(2, 1, 3)]
        [Ignore("Needs to be updated")]
        public void ShouldNotInsertLoadTimerWithJobPhaseCategoryMismatch(int jobID, int phaseID, int categoryID)
        {

            loadTimerDto = new LoadTimerDto
            {
                JobID = jobID,
                PhaseID = phaseID,
                CategoryID = categoryID
            };

            var builder = new LoadTimerServiceBuilder();
            var loadTimerService = builder.Build();

            builder.mockCategoryRepository.Setup(x => x.Find(categoryID))
                                        .Returns(categories.AsQueryable().FirstOrDefault(x => x.ID == categoryID));
            builder.mockPhaseRepository.Setup(x => x.Find(phaseID))
                                        .Returns(phases.AsQueryable().FirstOrDefault(x => x.ID == phaseID));
            builder.mockLoadTimerRepository.Setup(x => x.Insert(It.IsAny<LoadTimer>())).Returns((LoadTimer lt) => lt);

            // Act

            // Assert
            Assert.That(() => loadTimerService.Insert(loadTimerDto), Throws.TypeOf<ArgumentException>());
        }

        public List<Category> CreateCategories()
        {
            var category1 = new Category
            {
                ID = 1,
                PhaseID = 1,
                JobID = 1
            };
            var category2 = new Category
            {
                ID = 2,
                PhaseID = 2,
                JobID = 1
            };
            var category3 = new Category
            {
                ID = 3,
                PhaseID = 3,
                JobID = 2
            };
            return new List<Category> { category1, category2, category3 };
        }

        public List<Phase> CreatePhases()
        {
            var phase1 = new Phase
            {
                ID = 1,
                JobID = 1
            };
            var phase2 = new Phase
            {
                ID = 2,
                JobID = 1
            };
            var phase3 = new Phase
            {
                ID = 3,
                JobID = 2
            };
            return new List<Phase> { phase1, phase2, phase3 };

        }
    }
}