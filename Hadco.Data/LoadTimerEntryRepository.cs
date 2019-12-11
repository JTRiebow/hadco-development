using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;
using Hadco.Common.DataTransferObjects;
using Hadco.Data.Entities;

namespace Hadco.Data
{
    public class LoadTimerEntryRepository : GenericRepository<LoadTimerEntry>, ILoadTimerEntryRepository
    {
        public int GetTimesheetID(int loadTimerEntryID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                return sc.Query<int>(@"
                                 select lt.timesheetID
                                 from LoadTimerEntries lte
                                 join LoadTimers lt on lte.LoadTimerID = lt.LoadTimerID
                                 where lte.LoadTimerEntryID = @loadTimerEntryID
                                 ", new { loadTimerEntryID }).SingleOrDefault();
            }
        }

        public IEnumerable<BaseTimerDto> GetAllLoadTimerEntriesOnTimesheet(int timesheetID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                return sc.Query<BaseTimerDto>(@"
                                 select lte.StartTime, lte.EndTime
                                 from LoadTimerEntries lte
                                 join LoadTimers lt on lte.LoadTimerID = lt.LoadTimerID
                                 where lt.timesheetID = @timesheetID
                                 and lte.StartTime is not null
                                 and lte.EndTime is not null
                                 ", new { timesheetID });
            }
        }
    }

    public interface ILoadTimerEntryRepository : IGenericRepository<LoadTimerEntry>
    {
        int GetTimesheetID(int loadTimerEntryID);
        IEnumerable<BaseTimerDto> GetAllLoadTimerEntriesOnTimesheet(int timesheetID);
    }
}