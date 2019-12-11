using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Data;


namespace Hadco.Tests.Integration_Tests
{
    public class LoadTimerDataPopulator
    {
        HadcoContext Context = new HadcoContext();

        public int GetNewTimesheetID()
        {
            using (var connection = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                connection.Open();
                return connection.Query<int>(@" INSERT Timesheets (EmployeeID, EquipmentUseTime, [Day])
                                    VALUES (1, 0, GETDATE());
                                    SELECT SCOPE_IDENTITY()").Single();
            }
        }

        public void DeleteData()
        {
            using (var connection = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                connection.Open();
                connection.Query(@"");
            }
        }


    }
}
