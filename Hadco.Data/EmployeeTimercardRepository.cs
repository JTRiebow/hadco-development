using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Hadco.Common;
using Hadco.Common.DataTransferObjects;
using Hadco.Common.Enums;
using Hadco.Data.CsvObjects;

namespace Hadco.Data
{
    public class EmployeeTimecardRepository : GenericRepository<EmployeeTimecard>, IEmployeeTimecardRepository
    {
        public IEnumerable<TimecardWeeklySummaryDto> GetTimecardWeeklySummary(DateTime week, int currentUserId, int? supervisorID, int? departmentID, int? employeeID,
            string viewPermissionKey, bool? accountingApproved, bool? supervisorApproved, bool? billingApproved)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                var getEmployeeTimecardsQuery = Resources.GetEmployeeTimecards;
                var employeeTimecardIDs = sc.Query<int>(getEmployeeTimecardsQuery, new
                {
                    week,
                    currentUserId,
                    supervisorID,
                    departmentID,
                    employeeID,
                    viewPermissionKey,
                    accountingApproved = accountingApproved.HasValue ? Convert.ToInt16(accountingApproved) : (int?)null,
                    supervisorApproved = supervisorApproved.HasValue ? Convert.ToInt16(supervisorApproved) : (int?)null,
                    billingApproved = billingApproved.HasValue ? Convert.ToInt16(billingApproved) : (int?)null
                });

                var sql = Resources.GetWeeklyTimer;
                var gridReader = sc.QueryMultiple(sql, new { employeeTimecardIDs });

                var timecards = gridReader.Read<TimecardWeeklySummaryDto>().ToList();
                var dailyMap = gridReader.Read<EmployeeTimerSummary>()
                    .GroupBy(x => x.EmployeeTimecardID)
                    .ToDictionary(x => x.Key, x => x.AsEnumerable()); ;

                foreach (var timecard in timecards)
                {
                    IEnumerable<EmployeeTimerSummary> dailySummaries;
                    if (dailyMap.TryGetValue(timecard.EmployeeTimecardID, out dailySummaries))
                    {
                        timecard.Day0 = dailySummaries.SingleOrDefault(x => x.EmployeeTimecardID == timecard.EmployeeTimecardID && x.DayNumber == 0);
                        timecard.Day1 = dailySummaries.SingleOrDefault(x => x.EmployeeTimecardID == timecard.EmployeeTimecardID && x.DayNumber == 1);
                        timecard.Day2 = dailySummaries.SingleOrDefault(x => x.EmployeeTimecardID == timecard.EmployeeTimecardID && x.DayNumber == 2);
                        timecard.Day3 = dailySummaries.SingleOrDefault(x => x.EmployeeTimecardID == timecard.EmployeeTimecardID && x.DayNumber == 3);
                        timecard.Day4 = dailySummaries.SingleOrDefault(x => x.EmployeeTimecardID == timecard.EmployeeTimecardID && x.DayNumber == 4);
                        timecard.Day5 = dailySummaries.SingleOrDefault(x => x.EmployeeTimecardID == timecard.EmployeeTimecardID && x.DayNumber == 5);
                        timecard.Day6 = dailySummaries.SingleOrDefault(x => x.EmployeeTimecardID == timecard.EmployeeTimecardID && x.DayNumber == 6);
                    }

                }
                return timecards;
            }
        }

        public IEnumerable<EmployeeTimecardSummaryDto> GetEmployeeTimecardSummary(DateTime week, int employeeID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                return sc.Query<EmployeeTimecardSummaryDto>(Resources.GetEmployeeTimecardSummary, new { week, employeeID });
            }
        }

        public IEnumerable<TMCrushingTimecardCsvDto> GetTMCrushingEmployeeTimecardCsv(DateTime week)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                var departmentID = (int)DepartmentName.TMCrushing;
                sc.Open();
                return sc.Query<TMCrushingTimecardCsvDto>(Resources.GetEmployeeTimecardCsv, new { week, departmentID });
            }
        }
    }

    public interface IEmployeeTimecardRepository : IGenericRepository<EmployeeTimecard>
    {
        IEnumerable<TimecardWeeklySummaryDto> GetTimecardWeeklySummary(DateTime week, int currentUserId, int? supervisorID, int? departmentID, int? employeeID
            , string viewPermissionKey, bool? accountingApproved, bool? supervisorApproved, bool? billingApproved);
        IEnumerable<EmployeeTimecardSummaryDto> GetEmployeeTimecardSummary(DateTime week, int employeeID);
    }
}
