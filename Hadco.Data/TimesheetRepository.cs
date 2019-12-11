using Hadco.Data.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Hadco.Common.DataTransferObjects;
using Hadco.Data;

namespace Hadco.Data
{
    public class TimesheetRepository : GenericRepository<Timesheet>, ITimesheetRepository

    {
        public int? FindTimesheetID(int employeeID, DateTime day, int departmentID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                return sc.Query<int?>(@"
                            select TimesheetID
                            from Timesheets
                            where EmployeeID = @employeeID
                            and Day = @day
                            and departmentID = @departmentID", new {employeeID, day, departmentID}).SingleOrDefault();
            }
        }

        public int? GetOdometer(int employeeID, DateTime day, int departmentID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                return sc.Query<int?>(@"
                        select Odometer
                        from Timesheets
                        where employeeID = @employeeID
                        and day = @day
                        and departmentID = @departmentID;", new { employeeID, day, departmentID }).FirstOrDefault();
            }
        }

        public IEnumerable<SuperintendentTimesheetsDto> GetForemanTimesheetsFromEmployee(int employeeID, DateTime week)
        {
            return GetForemenTimesheets(week, Resources.GetSuperintendentForemanTimesheetsWithEmployee, employeeID: employeeID);
        }

        public IEnumerable<SuperintendentTimesheetsDto> GetForemenTimesheetsForSupervisor(int superintendentID, DateTime week)
        {
            return GetForemenTimesheets(week, Resources.GetSuperintendentForemanTimesheets, superintendentID: superintendentID);
        }

        private IEnumerable<SuperintendentTimesheetsDto> GetForemenTimesheets(DateTime week, string query, int? superintendentID = null, int? employeeID = null)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                var reader = sc.QueryMultiple(query, new { superintendentID, employeeID, week });
                var superintendentTimesheetsDtos = reader.Read<SuperintendentTimesheetsDto>().ToList();
                var foremanTimesheets = reader.Read<ForemanTimesheetDto>().ToList();
                var jobNumbers = reader.Read().ToList();

                foreach (var timesheet in foremanTimesheets)
                {
                    timesheet.JobNumbers = jobNumbers.Where(x => (int)x.TimesheetID == timesheet.TimesheetID).Select(x => (string)x.JobNumber);
                }

                foreach (var timesheet in superintendentTimesheetsDtos)
                {
                    timesheet.Timesheets = foremanTimesheets.Where(x => x.EmployeeID == timesheet.EmployeeID);
                }
                return superintendentTimesheetsDtos;
            }
        }
    }

    public interface ITimesheetRepository : IGenericRepository<Timesheet>
    {
        int? FindTimesheetID(int employeeID, DateTime day, int departmentID);
        IEnumerable<SuperintendentTimesheetsDto> GetForemenTimesheetsForSupervisor(int superintendentID, DateTime week);
        IEnumerable<SuperintendentTimesheetsDto> GetForemanTimesheetsFromEmployee(int employeeID, DateTime week);
        int? GetOdometer(int employeeID, DateTime day, int departmentID);
    }
}
