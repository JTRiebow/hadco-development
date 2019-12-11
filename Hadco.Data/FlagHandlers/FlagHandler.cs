using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Common.Enums;

namespace Hadco.Data.FlagHandlers
{
    public abstract class FlagHandler
    {
        public abstract NoteTypeName FlagType { get; }

        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["Hadco.Data.HadcoContext"].ConnectionString;

        public abstract Task<bool> MeetsCondition(int employeeID, DateTime day, int departmentID);

        public virtual async Task ResolveFlagIfExists(int employeeID, DateTime day, int departmentID, int currentEmployeeID)
        {
            using (var sc = new SqlConnection(_connectionString))
            {
                await sc.OpenAsync();
                await sc.ExecuteAsync(@"
update Notes 
    set Resolved = 1, 
        ResolvedTime = getdate(), 
        ResolvedEmployeeID = @currentEmployeeID,
        ModifiedTime = getdate()
    where EmployeeID = @employeeID
        and Day = @day
        and DepartmentID = @departmentID
        and NoteTypeID = @flagType 
        and Resolved = 0",
                    new { employeeID, day, departmentID, flagType = (int)FlagType, currentEmployeeID });
            }
            ;
        }
        public virtual async Task CreateOrUpdateFlag(int employeeID, DateTime day, int departmentID, string description)
        {
            using (var sc = new SqlConnection(_connectionString))
            {
                await sc.OpenAsync();
                await sc.ExecuteAsync(@"
declare @systemEmployeeId int = (select top 1 EmployeeID from Employees where Username = 'system');
if exists (
    select * from Notes
        where EmployeeID = @employeeID
        and Day = @day
        and DepartmentID = @departmentID
        and NoteTypeID = @flagType
    )
    update Notes 
        set Resolved = 0
    where EmployeeID = @employeeID
        and Day = @day
        and DepartmentID = @departmentID
        and NoteTypeID = @flagType 
else
    insert into Notes (Description, EmployeeID, Day, DepartmentID, Resolved, ModifiedTime, NoteTypeID, CreatedTime, CreatedEmployeeID) 
        values (@description, @employeeID, @day, @departmentID, 0, getdate(), @flagType, getdate(), @systemEmployeeID)",
                    new { employeeID, day, departmentID, flagType = (int)FlagType, description });
            }
            ;
        }

        internal async Task<bool> RunConditionQuery(int employeeID, DateTime day, int departmentID, string conditionQuery)
        {
            using (var sc = new SqlConnection(_connectionString))
            {
                await sc.OpenAsync();
                return await sc.ExecuteScalarAsync<bool>(conditionQuery, new { employeeID, day, departmentID });
            }
        }
    }
}
