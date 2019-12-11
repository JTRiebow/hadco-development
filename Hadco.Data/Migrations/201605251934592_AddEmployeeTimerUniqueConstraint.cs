namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployeeTimerUniqueConstraint : DbMigration
    {
        public override void Up()
        {
            Sql(@"if object_id('[UC_dbo.EmployeeTimers_EmployeeID_Day_DepartmentID]') is null
                    ALTER TABLE[dbo].[EmployeeTimers]  WITH NOCHECK ADD CONSTRAINT[UC_dbo.EmployeeTimers_EmployeeID_Day_DepartmentID] UNIQUE(EmployeeID, Day, DepartmentID);");
        }
        
        public override void Down()
        {
        }
    }
}
