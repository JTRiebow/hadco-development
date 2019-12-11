using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Data.Entities;

namespace Hadco.Data
{
    public class SettingRepository : GenericRepository<Setting>, ISettingRepository
    {
        public int GetBreadCrumbIntervalInSeconds()
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                return sc.Query<int>(@"
                    select top 1 BreadCrumbSeconds
                    from Settings
                    where ModifiedTime < sysdatetimeoffset()
                    order by ModifiedTime desc").FirstOrDefault();
            }
        }
    }

    public interface ISettingRepository : IGenericRepository<Setting>
    {
        int GetBreadCrumbIntervalInSeconds();
    }
}
