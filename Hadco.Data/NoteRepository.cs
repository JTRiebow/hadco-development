using Hadco.Common.Enums;
using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using Dapper;
using Hadco.Data.FlagHandlers;

namespace Hadco.Data
{
    public class NoteRepository : GenericRepository<Note>, INoteRepository
    {
        public void TriggerFlag(NoteTypeName type, int employeeID, DateTime day, int departmentID, int modifierID)
        {
            HostingEnvironment.QueueBackgroundWorkItem(ct => GenerateFlag(type, employeeID, day, departmentID, modifierID));
        }

        private async Task GenerateFlag(NoteTypeName type, int employeeID, DateTime day, int departmentID, int modifierID)
        {
            FlagHandler flagHandler;
            switch (type)
            {
                case NoteTypeName.TooManyHours:
                    flagHandler = new TooManyHoursFlagHandler();
                    break;
                case NoteTypeName.Injury:
                    flagHandler = new InjuryFlagHandler();
                    break;
                case NoteTypeName.InvalidClockOut:
                    flagHandler = new NullFlagHandler();
                    break;
                default:
                    flagHandler = new NullFlagHandler();
                    break;
            }
            var meetsCondition = await flagHandler.MeetsCondition(employeeID, day, departmentID);
            if (meetsCondition)
            {
                await flagHandler.CreateOrUpdateFlag(employeeID, day, departmentID, $"System Generated Flag: {type}");
            }
            else
            {
                await flagHandler.ResolveFlagIfExists(employeeID, day, departmentID, modifierID);
            }
        }

    }

    public interface INoteRepository : IGenericRepository<Note>
    {
        void TriggerFlag(NoteTypeName type, int employeeID, DateTime day, int departmentID, int modifierID);
    }
}
