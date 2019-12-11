namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateApprovalLevel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmployeeTimecards", "ApprovedByAccountingEmployeeID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeTimecards", "ApprovedBySupervisorEmployeeID", "dbo.Employees");
            DropIndex("dbo.EmployeeTimecards", new[] { "ApprovedBySupervisorEmployeeID" });
            DropIndex("dbo.EmployeeTimecards", new[] { "ApprovedByAccountingEmployeeID" });
            AddColumn("dbo.EmployeeTimers", "ApprovedBySupervisor", c => c.Boolean(nullable: false));
            AddColumn("dbo.EmployeeTimers", "ApprovedBySupervisorEmployeeID", c => c.Int());
            AddColumn("dbo.EmployeeTimers", "ApprovedBySupervisorTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EmployeeTimers", "ApprovedByAccounting", c => c.Boolean(nullable: false));
            AddColumn("dbo.EmployeeTimers", "ApprovedByAccountingEmployeeID", c => c.Int());
            AddColumn("dbo.EmployeeTimers", "ApprovedByAccountingTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EmployeeTimers", "ApprovedByBillingEmployeeID", c => c.Int());
            AddColumn("dbo.EmployeeTimers", "ApprovedByBillingTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EmployeeTimers", "Flagged", c => c.Boolean(nullable: false));

            Sql(@"update et 
set et.ApprovedBySupervisor = etc.ApprovedBySupervisor,
et.ApprovedBySupervisorEmployeeID = etc.ApprovedBySupervisorEmployeeID,
et.ApprovedBySupervisorTime = etc.ApprovedBySupervisorTime, 
et.ApprovedByAccounting = etc.ApprovedByAccounting, 
et.ApprovedByAccountingEmployeeID = etc.ApprovedByAccountingEmployeeID,
et.ApprovedByAccountingTime = etc.ApprovedByAccountingTime,
et.Flagged = etc.Flagged
from EmployeeTimers et
join EmployeeTimecards etc on et.EmployeeTimecardID = etc.EmployeeTimecardID");
            CreateIndex("dbo.EmployeeTimers", "ApprovedBySupervisorEmployeeID");
            CreateIndex("dbo.EmployeeTimers", "ApprovedByAccountingEmployeeID");
            CreateIndex("dbo.EmployeeTimers", "ApprovedByBillingEmployeeID");
            AddForeignKey("dbo.EmployeeTimers", "ApprovedByAccountingEmployeeID", "dbo.Employees", "EmployeeID");
            AddForeignKey("dbo.EmployeeTimers", "ApprovedByBillingEmployeeID", "dbo.Employees", "EmployeeID");
            AddForeignKey("dbo.EmployeeTimers", "ApprovedBySupervisorEmployeeID", "dbo.Employees", "EmployeeID");
            DropColumn("dbo.EmployeeTimecards", "ApprovedBySupervisor");
            DropColumn("dbo.EmployeeTimecards", "ApprovedBySupervisorEmployeeID");
            DropColumn("dbo.EmployeeTimecards", "ApprovedBySupervisorTime");
            DropColumn("dbo.EmployeeTimecards", "ApprovedByAccounting");
            DropColumn("dbo.EmployeeTimecards", "ApprovedByAccountingEmployeeID");
            DropColumn("dbo.EmployeeTimecards", "ApprovedByAccountingTime");
            DropColumn("dbo.EmployeeTimecards", "Flagged");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmployeeTimecards", "Flagged", c => c.Boolean(nullable: false));
            AddColumn("dbo.EmployeeTimecards", "ApprovedByAccountingTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EmployeeTimecards", "ApprovedByAccountingEmployeeID", c => c.Int());
            AddColumn("dbo.EmployeeTimecards", "ApprovedByAccounting", c => c.Boolean(nullable: false));
            AddColumn("dbo.EmployeeTimecards", "ApprovedBySupervisorTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EmployeeTimecards", "ApprovedBySupervisorEmployeeID", c => c.Int());
            AddColumn("dbo.EmployeeTimecards", "ApprovedBySupervisor", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.EmployeeTimers", "ApprovedBySupervisorEmployeeID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeTimers", "ApprovedByBillingEmployeeID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeTimers", "ApprovedByAccountingEmployeeID", "dbo.Employees");
            DropIndex("dbo.EmployeeTimers", new[] { "ApprovedByBillingEmployeeID" });
            DropIndex("dbo.EmployeeTimers", new[] { "ApprovedByAccountingEmployeeID" });
            DropIndex("dbo.EmployeeTimers", new[] { "ApprovedBySupervisorEmployeeID" });
            DropColumn("dbo.EmployeeTimers", "Flagged");
            DropColumn("dbo.EmployeeTimers", "ApprovedByBillingTime");
            DropColumn("dbo.EmployeeTimers", "ApprovedByBillingEmployeeID");
            DropColumn("dbo.EmployeeTimers", "ApprovedByAccountingTime");
            DropColumn("dbo.EmployeeTimers", "ApprovedByAccountingEmployeeID");
            DropColumn("dbo.EmployeeTimers", "ApprovedByAccounting");
            DropColumn("dbo.EmployeeTimers", "ApprovedBySupervisorTime");
            DropColumn("dbo.EmployeeTimers", "ApprovedBySupervisorEmployeeID");
            DropColumn("dbo.EmployeeTimers", "ApprovedBySupervisor");
            CreateIndex("dbo.EmployeeTimecards", "ApprovedByAccountingEmployeeID");
            CreateIndex("dbo.EmployeeTimecards", "ApprovedBySupervisorEmployeeID");
            AddForeignKey("dbo.EmployeeTimecards", "ApprovedBySupervisorEmployeeID", "dbo.Employees", "EmployeeID");
            AddForeignKey("dbo.EmployeeTimecards", "ApprovedByAccountingEmployeeID", "dbo.Employees", "EmployeeID");
        }
    }
}
