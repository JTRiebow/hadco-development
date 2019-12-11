namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeeTimecardApprovalsByAndTimes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeTimecards", "ApprovedBySupervisorEmployeeID", c => c.Int());
            AddColumn("dbo.EmployeeTimecards", "ApprovedBySupervisorTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EmployeeTimecards", "ApprovedByAccountingEmployeeID", c => c.Int());
            AddColumn("dbo.EmployeeTimecards", "ApprovedByAccountingTime", c => c.DateTimeOffset(precision: 7));
            CreateIndex("dbo.EmployeeTimecards", "ApprovedBySupervisorEmployeeID");
            CreateIndex("dbo.EmployeeTimecards", "ApprovedByAccountingEmployeeID");
            AddForeignKey("dbo.EmployeeTimecards", "ApprovedByAccountingEmployeeID", "dbo.Employees", "EmployeeID");
            AddForeignKey("dbo.EmployeeTimecards", "ApprovedBySupervisorEmployeeID", "dbo.Employees", "EmployeeID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeTimecards", "ApprovedBySupervisorEmployeeID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeTimecards", "ApprovedByAccountingEmployeeID", "dbo.Employees");
            DropIndex("dbo.EmployeeTimecards", new[] { "ApprovedByAccountingEmployeeID" });
            DropIndex("dbo.EmployeeTimecards", new[] { "ApprovedBySupervisorEmployeeID" });
            DropColumn("dbo.EmployeeTimecards", "ApprovedByAccountingTime");
            DropColumn("dbo.EmployeeTimecards", "ApprovedByAccountingEmployeeID");
            DropColumn("dbo.EmployeeTimecards", "ApprovedBySupervisorTime");
            DropColumn("dbo.EmployeeTimecards", "ApprovedBySupervisorEmployeeID");
        }
    }
}
