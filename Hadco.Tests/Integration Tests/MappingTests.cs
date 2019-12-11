using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Geocoding;
using Hadco.Data.Entities;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Hadco.Tests.Integration_Tests
{
    [TestFixture]
    public class MappingTests
    {
        [Test]
        public void MappingSetUpNoExceptions()
        {
            MappingProfile.Configure();
            //Not strictly a unit test, but this was useful to write for development
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var timesheet = fixture.Create<Timesheet>();
            Assert.DoesNotThrow(() => Mapper.Map<ExpandedTimesheetDto>(timesheet));
        }

        [Test]
        public void MappingSetUpNoExceptionsNoEquipmentTimers()
        {
            MappingProfile.Configure();
            //Not strictly a unit test, but this was useful to write for development
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var timesheet = fixture.Build<Timesheet>()
                .Without(x=>x.EquipmentTimers)
                .Create();
            
            Assert.DoesNotThrow(() => Mapper.Map<ExpandedTimesheetDto>(timesheet));
        }

        [Test]
        public void MappingSetUpWithEquipmentTimers()
        {
            MappingProfile.Configure();
            //Not strictly a unit test, but this was useful to write for development
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var timesheet = fixture.Build<Timesheet>()
                                   .With(x => x.EquipmentTimers)
                                   .Create();

            var dto = Mapper.Map<ExpandedTimesheetDto>(timesheet);
            Assert.That(dto.EquipmentTimers != null);
        }

        [Test]
        public void MappingSetUpNoEquipmentTimers()
        {
            MappingProfile.Configure();
            //Not strictly a unit test, but this was useful to write for development
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var timesheet = fixture.Build<Timesheet>()
                                   .Without(x => x.EquipmentTimers)
                                   .Create();

            var dto = Mapper.Map<ExpandedTimesheetDto>(timesheet);
            Assert.That(dto.EquipmentTimers == null || dto.EquipmentTimers.Count == 0);
        }

        [Test]
        public void MappingSetUpNoEquipmentTimerEntries()
        {
            MappingProfile.Configure();
            //Not strictly a unit test, but this was useful to write for development
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var timesheet = fixture.Build<Timesheet>()
                                   .With(x => x.EquipmentTimers)
                                   .Create();
            timesheet.EquipmentTimers.ForEach(x=>x.EquipmentTimerEntries = null);

            var dto = Mapper.Map<ExpandedTimesheetDto>(timesheet);
            Assert.That(dto.EquipmentTimers != null);
        }
    }
}
