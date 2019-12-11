using System;
using System.Threading.Tasks;
using Hadco.Common.Enums;

namespace Hadco.Data.FlagHandlers
{
    public class InjuryFlagHandler : FlagHandler
    {
        public override NoteTypeName FlagType { get; } = NoteTypeName.Injury;

        public override async Task<bool> MeetsCondition(int employeeID, DateTime day, int departmentID)
        {
            var conditionQuery = @"
select Injured
from EmployeeTimers
where EmployeeID = @employeeID
    and Day = @day
    and DepartmentID = @departmentID";
            return await RunConditionQuery(employeeID, day, departmentID, conditionQuery);
        }

    }
}
