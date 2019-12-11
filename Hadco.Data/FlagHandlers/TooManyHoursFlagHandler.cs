using System;
using System.Threading.Tasks;
using Hadco.Common.Enums;

namespace Hadco.Data.FlagHandlers
{
    public class TooManyHoursFlagHandler : FlagHandler
    {
        public override NoteTypeName FlagType { get; } = NoteTypeName.TooManyHours;

        public override async Task<bool> MeetsCondition(int employeeID, DateTime day, int departmentID)
        {
            var conditionQuery = @"
select case when sum(dbo.GetTimeSpanHours(ete.ClockIn, ete.ClockOut)) > 15 then 1 else 0 end
    from EmployeeTimers et 
    left join EmployeeTimerEntries ete on ete.EmployeeTimerID = et.EmployeeTimerID
where EmployeeID = @employeeID
    and Day = @day";

            return await RunConditionQuery(employeeID, day, departmentID, conditionQuery);
        }
    }
}
