using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq.Expressions;
using Dapper;
using Hadco.Common.DataTransferObjects;
using Hadco.Data.Entities;
using System.Linq;
using Hadco.Common;

namespace Hadco.Data
{
    public class LoadTimerRepository : GenericRepository<LoadTimer>, ILoadTimerRepository
    {
        public IEnumerable<TruckerDailyDto> GetTruckerDailies(DateTime startDate, DateTime endDate, int? departmentID)
        {
            var filter = @" where t.Day between @startDate and @endDate
                            and t.DepartmentID = @departmentID ";
            var query = Resources.GetTruckerDailiesLoadTimers + filter + " union all " +
                        Resources.GetTruckerDailiesDowntimeTimers + filter;
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                return sc.Query<TruckerDailyDto>(query, new {startDate, endDate, departmentID});
            }
        }

        public TruckerDailyDto GetLoadTruckerDaily(int loadTimerID)
        {
            var filter = @" where lt.LoadTimerID = @loadTimerID";
            var query = Resources.GetTruckerDailiesLoadTimers + filter;
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                return sc.Query<TruckerDailyDto>(query, new {loadTimerID}).FirstOrDefault();
            }
        }

        public override LoadTimer FindNoTracking(int id, params Expression<Func<LoadTimer, object>>[] includeProperties)
        {
            return base.FindNoTracking(id, x => x.LoadTimerEntries);
        }

        public void UpdatePricePerUnit(int customerTypeID, int billTypeID, int? jobID, int? customerID, int? phaseID,
            int? materialID, int? truckClassificationID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                sc.Query(Resources.UpdatePricePerUnit,
                    new {customerTypeID, billTypeID, jobID, customerID, phaseID, materialID, truckClassificationID});
            }
        }
    }

    public interface ILoadTimerRepository : IGenericRepository<LoadTimer>
    {
        IEnumerable<TruckerDailyDto> GetTruckerDailies(DateTime startDate, DateTime endDate, int? departmentID);
        TruckerDailyDto GetLoadTruckerDaily(int loadTimerID);

        void UpdatePricePerUnit(int customerTypeID, int billTypeID, int? jobID, int? customerID, int? phaseID,
            int? materialID, int? truckClassificationID);
    }
}