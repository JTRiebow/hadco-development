using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Common.DataTransferObjects;
using Hadco.Common;
using Hadco.Common.Enums;

namespace Hadco.Data
{
    public class JobTimerRepository : GenericRepository<JobTimer>, IJobTimerRepository
    {

        public void AddMechanicDowntime(int timesheetID)
        {
            var timesheet = Context.Timesheets.Find(timesheetID);
            if (timesheet.DepartmentID != (int)DepartmentName.Mechanic)
            {
                return;
            }
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Execute(Resources.AddMechanicDowntime, new { TimesheetID = timesheetID });
            }
        }

        public Quantities GetQuantities(int jobTimerID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                var mapped = sc.Query<Quantities>(@"select dbo.GetPreviousQuantity(@jobTimerID) PreviousQuantity, dbo.GetOtherNewQuantity(@jobTimerID) OtherNewQuantity;", new { jobTimerID }).FirstOrDefault();
                return mapped;
            }
        }
    }

    public interface IJobTimerRepository : IGenericRepository<JobTimer>
    {
        void AddMechanicDowntime(int timesheetID);

        Quantities GetQuantities(int jobTimerID);
    }
}
