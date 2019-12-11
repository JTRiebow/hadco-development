using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Common.DataTransferObjects;
using Hadco.Data.Entities;

namespace Hadco.Data
{
    public class DailyApprovalRepository : GenericRepository<DailyApproval>, IDailyApprovalRepository
    {

        public DailyApproval Get(int employeeID, DateTime day, int departmentID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                return sc.Query<DailyApproval>(@"
select top 1 *
from DailyApprovals
where EmployeeID = @employeeID
and Day = @day
and DepartmentID = @departmentID", new { employeeID, day, departmentID }).FirstOrDefault();
            }
        }

    }

    public interface IDailyApprovalRepository : IGenericRepository<DailyApproval>
    {
        DailyApproval Get(int employeeID, DateTime day, int departmentID);
    }
}
