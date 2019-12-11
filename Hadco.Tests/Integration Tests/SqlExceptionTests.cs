using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Hadco.Common.Enums;
using Hadco.Data;
using NUnit.Framework;
using Hadco.Common.DataTransferObjects;

namespace Hadco.Tests.Integration_Tests
{
    [TestFixture]
    public class SqlExceptionTests
    {
        private HadcoContext _context;
        protected HadcoContext Context
        {
            get
            {
                return _context = _context ?? new HadcoContext();
            }
        }


        [Test]
        [Ignore("Needs to be updated")]
        public void GetWeeklyTimers_NoExceptions()
        {
            var week = DateTimeOffset.Now.AddDays(-10);

            using (var connection = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                connection.Open();
                Assert.DoesNotThrow(() => connection.Query(Resources.GetWeeklyTimer, new { week }));
            }
        }

        //[Test]
        //public void GetTruckerDailies_NoExceptions()
        //{
        //    var startDate = DateTimeOffset.Now.AddDays(-10).AddHours(-2);
        //    var endDate = DateTimeOffset.Now.AddDays(-10);

        //    using (var connection = new SqlConnection(Context.Database.Connection.ConnectionString))
        //    {
        //        connection.Open();
        //        Assert.DoesNotThrow(() => connection.Query(Resources.GetTruckerDailies, new { startDate, endDate , departmentID = DepartmentName.Trucking}));
        //    }
        //}

        //[Test]
        //public void GetSingleTruckerDaily_NoExceptions()
        //{
        //    int loadTimerID = 1;

        //    using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
        //    {
        //        sc.Open();
        //        Assert.DoesNotThrow(() => sc.Query<TruckerDailyDto>(Resources.GetSingleTruckerDaily, new { loadTimerID }));
        //    }
        //}


                    

        [Test]
        [Ignore("Needs to be updated")]
        public void AddLoadTimerDowntimes_NoExceptions()
        {
            int timesheetID;
            using (var connection = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                connection.Open();
                timesheetID = connection.Query<int>(@"select top(1) t.timesheetID
                                                      from Timesheets t
                                                      join LoadTimers lt on t.TimesheetID = lt.TimesheetID
                                                      join LoadTimerEntries lte on lt.LoadTimerID = lte.LoadTimerID
                                                      join DowntimeTimers dt on lt.LoadTimerID = dt.LoadTimerID
                                                      where lte.EndTime is not null
                                                      order by t.day desc").FirstOrDefault();
            }
            using (var connection = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                connection.Open();
                Assert.DoesNotThrow(() => connection.Query(Resources.AddLoadTimerDowntimes, new {timesheetID}));
            }
        }

        [Test]
        [TestCase(DepartmentName.Concrete)]
        [TestCase(DepartmentName.Residential)]
        [TestCase(DepartmentName.Development)]
        [TestCase(DepartmentName.FrontOffice)]
        [TestCase(DepartmentName.Mechanic)]
        [TestCase(DepartmentName.TMCrushing)]
        [TestCase(DepartmentName.ConcreteHB)]
        [TestCase(DepartmentName.Concrete2H)]
        [TestCase(DepartmentName.Trucking)]
        [Ignore("Needs to be updated")]
        public void GetEmployeeTimecardCsv_NoExceptions(int departmentID)
        {
            var week = DateTimeOffset.Now.AddDays(-10);
            using (var connection = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                connection.Open();
                Assert.DoesNotThrow(() => connection.Query(Resources.GetEmployeeTimecardCsv, new { week, departmentID }));
            }
        }

        [Test]
        [TestCase(DepartmentName.Concrete)]
        [TestCase(DepartmentName.Concrete2H)]
        [TestCase(DepartmentName.ConcreteHB)]
        [Ignore("Needs to be updated")]
        public void GetConcreteJobTimerCsv_NoExceptions(int departmentID)
        {
            var week = DateTimeOffset.Now.AddDays(-10);

            using (var connection = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                connection.Open();
                Assert.DoesNotThrow(() => connection.Query(Resources.GetJobTimerCsv, new { departmentID, week }));
            }
        }
    }
}