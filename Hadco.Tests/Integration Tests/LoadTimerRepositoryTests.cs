using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Moq;
using NUnit.Framework;

namespace Hadco.Tests.Integration_Tests
{
    [TestFixture]
    public class LoadTimerRepositoryTests
    {
        public LoadTimerRepositoryTests()
        {

            MappingProfile.Configure();
        }

        //[Test]
        //public void LoadTimerRepo_AddOldLoadTimer()
        //{
        //    // Arrange
        //    var dataPopulator = new LoadTimerDataPopulator();
        //    var builder = new LoadTimerRepositoryBuilder();
        //    var loadTimerRepository = builder.Build();

        //    var timesheetID = dataPopulator.GetNewTimesheetID();

        //    var loadTime = DateTimeOffset.Now.AddHours(-5);
        //    var dumpTime = DateTimeOffset.Now;

        //    var loadTimer = new LoadTimer()
        //    {
        //        TimesheetID = timesheetID,
        //        LoadTimerEntries = new List<LoadTimerEntry> {
        //            new LoadTimerEntry
        //            {
        //                StartTime = DateTimeOffset.Now.AddHours(-4),
        //                EndTime = DateTimeOffset.Now
        //            }}
        //    };

        //    // Act

        //    var result = loadTimerRepository.Insert(loadTimer);


        //    // Assert

        //    var loadTimerID = result.ID;
        //    var test = loadTimerRepository.FindNoTracking(loadTimerID);

        //    Assert.AreEqual(result, test);
        //}
    }
}
