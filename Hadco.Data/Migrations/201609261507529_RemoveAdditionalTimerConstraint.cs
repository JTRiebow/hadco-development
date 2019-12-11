namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAdditionalTimerConstraint : DbMigration
    {
        public override void Up()
        {
            Sql(@"if object_id('[UK_dbo.EmployeeTimers_EmployeeID_Day_DepartmentID]') is not null
                    ALTER TABLE [dbo].[EmployeeTimers] DROP CONSTRAINT [UK_dbo.EmployeeTimers_EmployeeID_Day_DepartmentID];");
        }

        public override void Down()
        {
            Sql(@"if object_id('[UK_dbo.EmployeeTimers_EmployeeID_Day_DepartmentID]') is null
                    ALTER TABLE[dbo].[EmployeeTimers]  WITH NOCHECK ADD CONSTRAINT[UK_dbo.EmployeeTimers_EmployeeID_Day_DepartmentID] UNIQUE(EmployeeID, Day, DepartmentID);");
        }
    }
}
