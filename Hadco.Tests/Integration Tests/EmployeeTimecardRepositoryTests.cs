using System;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using Dapper;
using Hadco.Data;
using Hadco.Data.Migrations;
using NUnit.Framework;

namespace Hadco.Tests.Integration_Tests
{
    [TestFixture]
    public class EmployeeTimecardRepositoryTests
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
        public void GetGetTMCrushingEmployeeTimecardCsv_ReturnsValues()
        {
            // Arrange
            var employeeTimecardRepository = new EmployeeTimecardRepository();
            var week = DateTime.Now.AddMonths(1);
            var departmentID = 7;
            int employeeID;

            using (var connection = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                //Adds an new employee and employee timer 1 month from now for 5 hours.
                connection.Open();
                employeeID = connection.Query<int>(@"declare @EmployeeID int, @TimesheetID int, @EmployeeTimecardID int, @EmployeeTimerID int, @PitID int;
                                        
                                        insert into Pits (Name)
                                        values ('TEST PIT');
                                        set @PitID = SCOPE_IDENTITY();

                                        insert into Employees ([Status], Username, [Password], EmployeeTypeID, OriginComputerEase)
                                        values (1, CONVERT(varchar(64), NEWID()), 'unhashedPassword', 1, 0)
                                        set @EmployeeID = SCOPE_IDENTITY(); 
                                        
                                        insert into Timesheets (EmployeeID, EquipmentUseTime, [Day], DepartmentID)
                                        values (@EmployeeID, 0, @week, @departmentID)
                                        set @TimesheetID = SCOPE_IDENTITY();
                                        
                                        insert into EmployeeTimecards (EmployeeID, DepartmentID, StartOfWeek, ApprovedBySupervisor, ApprovedByAccounting, Flagged)
                                        values (@EmployeeID, @departmentID, @week, 1, 1, 0)
                                        set @EmployeeTimecardID = SCOPE_IDENTITY();
                                        
                                        insert into EmployeeTimers (EmployeeID, EmployeeTimecardID, DepartmentID, [Day], TimesheetID, Injured, EquipmentUseTime)
                                        values (@EmployeeID, @EmployeeTimecardID, @departmentID, @week, @TimesheetID, 0, 0)
                                        set @EmployeeTimerID = SCOPE_IDENTITY();
                                        
                                        insert into EmployeeTimerEntries (EmployeeTimerID, ClockIn, ClockOut, PitID)
                                        values (@EmployeeTimerID, DATEADD(HOUR, -3, @week), DATEADD(HOUR, 2, @week), @PitID);
                                        
                                        select @EmployeeID", new { week, departmentID }).Single();
            }

            // Act
            var results = employeeTimecardRepository.GetTMCrushingEmployeeTimecardCsv(week);
            using (var connection = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                connection.Open();
                connection.Query(@" delete from EmployeeTimerEntries where Clockout > @week;
                                    delete from EmployeeTimers where EmployeeID = @employeeID;
                                    delete from EmployeeTimecards where EmployeeID = @employeeID;
                                    delete from Timesheets where EmployeeID = @employeeID;
                                    delete from employees where EmployeeID = @employeeID;
                                    delete from Pits where Name = 'TEST PIT'", new { week, employeeID });
            }

            // Assert
            Assert.GreaterOrEqual(results.Count(x => x.TotalHours == 5 && x.Pit == "TEST PIT" ), 1);

        }
    }

}
