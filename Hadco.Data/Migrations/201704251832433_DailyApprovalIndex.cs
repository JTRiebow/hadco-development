namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DailyApprovalIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DailyApprovals", new[] { "EmployeeID" });
            DropIndex("dbo.DailyApprovals", new[] { "DepartmentID" });
            CreateIndex("dbo.DailyApprovals", new[] { "EmployeeID", "Day", "DepartmentID" }, unique: true, name: "IX_EmployeeDayDepartmentApproval");
        }
        
        public override void Down()
        {
            DropIndex("dbo.DailyApprovals", "IX_EmployeeDayDepartmentApproval");
            CreateIndex("dbo.DailyApprovals", "DepartmentID");
            CreateIndex("dbo.DailyApprovals", "EmployeeID");
        }
    }
}
