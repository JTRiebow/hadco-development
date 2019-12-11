using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Dapper;

    public partial class AddUniqueFilteredConstraints : DbMigration
    {
        public override void Up()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Hadco.Data.HadcoContext"].ConnectionString;
            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();
                var maxEmployeeTimerID = sc.ExecuteScalar<int>(@"select max(EmployeeTimerID) from EmployeeTimers");
                sc.Execute($@"IF EXISTS
                            (select * 
                            from sys.indexes 
                            where name = N'FIEmployeeTimersEmployeeIDDepartmentIDTimesheetIDDay')
                        DROP INDEX FIEmployeeTimersEmployeeIDDepartmentIDTimesheetIDDay ON [dbo].[EmployeeTimers];

                        CREATE UNIQUE NONCLUSTERED INDEX [FIEmployeeTimersEmployeeIDDepartmentIDTimesheetIDDay] ON [dbo].[EmployeeTimers]
                        (
                        	[EmployeeID] ASC,
                        	[DepartmentID] ASC,
                        	[TimesheetID] ASC,
                        	[Day] ASC
                        )
                        WHERE ([EmployeeTimerID]>{maxEmployeeTimerID})");

                var maxEmployeeJobEquipmentTimerID = sc.ExecuteScalar<int>(@"select max(EmployeeJobEquipmentTimerID) from EmployeeJobEquipmentTimers");
                sc.Execute($@"IF EXISTS
                            (select * 
                            from sys.indexes 
                            where name = N'FIEmployeeJobEquipmentTimersEmployeeJobTimerIDEquipmentID')
                        DROP INDEX FIEmployeeJobEquipmentTimersEmployeeJobTimerIDEquipmentID ON [dbo].[EmployeeJobEquipmentTimers];

                        CREATE UNIQUE NONCLUSTERED INDEX [FIEmployeeJobEquipmentTimersEmployeeJobTimerIDEquipmentID] ON [dbo].[EmployeeJobEquipmentTimers]
                        (
                        	[EmployeeJobTimerID] ASC,
                        	[EquipmentID] ASC
                        )
                        WHERE [EmployeeJobEquipmentTimerID] > {maxEmployeeJobEquipmentTimerID}");

                var maxEmployeeJobTimerID = sc.ExecuteScalar<int>(@"select max(EmployeeJobTimerID) from EmployeeJobTimers");
                sc.Execute($@"IF EXISTS
                            (select * 
                            from sys.indexes 
                            where name = N'FIEmployeeJobTimersEmployeeTimerIDJobTimerID')
                        DROP INDEX FIEmployeeJobTimersEmployeeTimerIDJobTimerID ON [dbo].[EmployeeJobTimers];

                        CREATE UNIQUE NONCLUSTERED INDEX [FIEmployeeJobTimersEmployeeTimerIDJobTimerID] ON [dbo].[EmployeeJobTimers]
                        (
                        	[EmployeeTimerID] ASC,
                        	[JobTimerID] ASC
                        )
                        WHERE [EmployeeJobTimerID] > {maxEmployeeJobTimerID}");
            }
        }

        public override void Down()
        {
        }
    }
}
