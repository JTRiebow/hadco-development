using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Common.DataTransferObjects;

namespace Hadco.Data
{
    public class DowntimeTimerRepository : GenericRepository<DowntimeTimer>, IDowntimeTimerRepository
    {
        public void AddLoadDowntimeTimers(int timesheetID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Execute(Resources.AddLoadTimerDowntimes, new { TimesheetID = timesheetID });
            }
        }
        public void DeleteDowntimeTimers(int loadTimerID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Execute("delete DowntimeTimers where LoadTimerID = @loadTimerID", new { LoadTimerID = loadTimerID });
            }
        }
        public TruckerDailyDto GetDowntimeTruckerDaily(int downtimeTimerID)
        {
            var filter = @" where d.DowntimeTimerID = @downtimeTimerID";
            var query = Resources.GetTruckerDailiesDowntimeTimers + filter;
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                return sc.Query<TruckerDailyDto>(query, new { downtimeTimerID }).FirstOrDefault();
            }
        }
    }

    public interface IDowntimeTimerRepository : IGenericRepository<DowntimeTimer>
    {
        void AddLoadDowntimeTimers(int timesheetID);
        void DeleteDowntimeTimers(int loadTimerID);
        TruckerDailyDto GetDowntimeTruckerDaily(int downtimeTimerID);
    }
}
