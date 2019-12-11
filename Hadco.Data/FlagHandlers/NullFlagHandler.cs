using System;
using System.Threading.Tasks;
using Hadco.Common.Enums;

namespace Hadco.Data.FlagHandlers
{
    public class NullFlagHandler : FlagHandler
    {
        public override NoteTypeName FlagType { get; } = NoteTypeName.Other;
        public override Task<bool> MeetsCondition(int employeeID, DateTime day, int departmentID)
        {
            return Task.FromResult(false);
        }

        public override Task ResolveFlagIfExists(int employeeID, DateTime day, int departmentID, int currentEmployeeID)
        {
            return Task.Delay(0);
        }

        public override Task CreateOrUpdateFlag(int employeeID, DateTime day, int departmentID, string description)
        {
            return Task.Delay(0);
        }
    }

}
