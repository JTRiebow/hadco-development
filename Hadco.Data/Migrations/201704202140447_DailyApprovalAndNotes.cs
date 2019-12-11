namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DailyApprovalAndNotes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmployeeTimers", "ApprovedByAccountingEmployeeID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeTimers", "ApprovedByBillingEmployeeID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeTimers", "ApprovedBySupervisorEmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Notes", "EmployeeTimerID", "dbo.EmployeeTimers");
            DropIndex("dbo.EmployeeTimers", new[] { "ApprovedBySupervisorEmployeeID" });
            DropIndex("dbo.EmployeeTimers", new[] { "ApprovedByAccountingEmployeeID" });
            DropIndex("dbo.EmployeeTimers", new[] { "ApprovedByBillingEmployeeID" });
            DropIndex("dbo.Notes", new[] { "EmployeeTimerID" });
            CreateTable(
                "dbo.DailyApprovals",
                c => new
                    {
                        DailyApprovalID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(nullable: false),
                        Day = c.DateTime(nullable: false, storeType: "date"),
                        DepartmentID = c.Int(nullable: false),
                        ApprovedBySupervisor = c.Boolean(nullable: false),
                        ApprovedBySupervisorEmployeeID = c.Int(),
                        ApprovedBySupervisorTime = c.DateTimeOffset(precision: 7),
                        ApprovedByBilling = c.Boolean(nullable: false),
                        ApprovedByBillingEmployeeID = c.Int(),
                        ApprovedByBillingTime = c.DateTimeOffset(precision: 7),
                        ApprovedByAccounting = c.Boolean(nullable: false),
                        ApprovedByAccountingEmployeeID = c.Int(),
                        ApprovedByAccountingTime = c.DateTimeOffset(precision: 7),
                    })
                .PrimaryKey(t => t.DailyApprovalID)
                .ForeignKey("dbo.Employees", t => t.ApprovedByAccountingEmployeeID)
                .ForeignKey("dbo.Employees", t => t.ApprovedByBillingEmployeeID)
                .ForeignKey("dbo.Employees", t => t.ApprovedBySupervisorEmployeeID)
                .ForeignKey("dbo.Departments", t => t.DepartmentID, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .Index(t => t.EmployeeID)
                .Index(t => t.DepartmentID)
                .Index(t => t.ApprovedBySupervisorEmployeeID)
                .Index(t => t.ApprovedByBillingEmployeeID)
                .Index(t => t.ApprovedByAccountingEmployeeID);
            
            AddColumn("dbo.Notes", "EmployeeID", c => c.Int(nullable: false));
            AddColumn("dbo.Notes", "Day", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.Notes", "DepartmentID", c => c.Int(nullable: false));

            Sql(@"update n set n.EmployeeID = et.EmployeeID, n.Day = et.Day, n.DepartmentID = et.DepartmentID
  from Notes n
  join EmployeeTimers et on n.EmployeeTimerID = et.EmployeeTimerID;");

            Sql(@"
insert into DailyApprovals 
(EmployeeID, Day, DepartmentID, 
ApprovedByAccountingTime, ApprovedByAccountingEmployeeID, ApprovedByAccounting, 
ApprovedBySupervisorTime, ApprovedBySupervisorEmployeeID, ApprovedBySupervisor, ApprovedByBilling)
select EmployeeID, Day, DepartmentID, 
min(ApprovedByAccountingTime), min(ApprovedByAccountingEmployeeID), min(cast(ApprovedByAccounting as int)),
min(ApprovedBySupervisorTime), min(ApprovedBySupervisorEmployeeID), min(cast(ApprovedBySupervisor as int)), 0
from EmployeeTimers
group by EmployeeID, Day, DepartmentID;");

            CreateIndex("dbo.Notes", "EmployeeID");
            CreateIndex("dbo.Notes", "DepartmentID");
            AddForeignKey("dbo.Notes", "DepartmentID", "dbo.Departments", "DepartmentID", cascadeDelete: true);
            AddForeignKey("dbo.Notes", "EmployeeID", "dbo.Employees", "EmployeeID", cascadeDelete: true);
            DropColumn("dbo.EmployeeTimers", "ApprovedBySupervisor");
            DropColumn("dbo.EmployeeTimers", "ApprovedBySupervisorEmployeeID");
            DropColumn("dbo.EmployeeTimers", "ApprovedBySupervisorTime");
            DropColumn("dbo.EmployeeTimers", "ApprovedByAccounting");
            DropColumn("dbo.EmployeeTimers", "ApprovedByAccountingEmployeeID");
            DropColumn("dbo.EmployeeTimers", "ApprovedByAccountingTime");
            DropColumn("dbo.EmployeeTimers", "ApprovedByBillingEmployeeID");
            DropColumn("dbo.EmployeeTimers", "ApprovedByBillingTime");
            DropColumn("dbo.Notes", "EmployeeTimerID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notes", "EmployeeTimerID", c => c.Int(nullable: false));
            AddColumn("dbo.EmployeeTimers", "ApprovedByBillingTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EmployeeTimers", "ApprovedByBillingEmployeeID", c => c.Int());
            AddColumn("dbo.EmployeeTimers", "ApprovedByAccountingTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EmployeeTimers", "ApprovedByAccountingEmployeeID", c => c.Int());
            AddColumn("dbo.EmployeeTimers", "ApprovedByAccounting", c => c.Boolean(nullable: false));
            AddColumn("dbo.EmployeeTimers", "ApprovedBySupervisorTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EmployeeTimers", "ApprovedBySupervisorEmployeeID", c => c.Int());
            AddColumn("dbo.EmployeeTimers", "ApprovedBySupervisor", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.Notes", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Notes", "DepartmentID", "dbo.Departments");
            DropForeignKey("dbo.DailyApprovals", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.DailyApprovals", "DepartmentID", "dbo.Departments");
            DropForeignKey("dbo.DailyApprovals", "ApprovedBySupervisorEmployeeID", "dbo.Employees");
            DropForeignKey("dbo.DailyApprovals", "ApprovedByBillingEmployeeID", "dbo.Employees");
            DropForeignKey("dbo.DailyApprovals", "ApprovedByAccountingEmployeeID", "dbo.Employees");
            DropIndex("dbo.Notes", new[] { "DepartmentID" });
            DropIndex("dbo.Notes", new[] { "EmployeeID" });
            DropIndex("dbo.DailyApprovals", new[] { "ApprovedByAccountingEmployeeID" });
            DropIndex("dbo.DailyApprovals", new[] { "ApprovedByBillingEmployeeID" });
            DropIndex("dbo.DailyApprovals", new[] { "ApprovedBySupervisorEmployeeID" });
            DropIndex("dbo.DailyApprovals", new[] { "DepartmentID" });
            DropIndex("dbo.DailyApprovals", new[] { "EmployeeID" });
            DropColumn("dbo.Notes", "DepartmentID");
            DropColumn("dbo.Notes", "Day");
            DropColumn("dbo.Notes", "EmployeeID");
            DropTable("dbo.DailyApprovals");
            CreateIndex("dbo.Notes", "EmployeeTimerID");
            CreateIndex("dbo.EmployeeTimers", "ApprovedByBillingEmployeeID");
            CreateIndex("dbo.EmployeeTimers", "ApprovedByAccountingEmployeeID");
            CreateIndex("dbo.EmployeeTimers", "ApprovedBySupervisorEmployeeID");
            AddForeignKey("dbo.Notes", "EmployeeTimerID", "dbo.EmployeeTimers", "EmployeeTimerID", cascadeDelete: true);
            AddForeignKey("dbo.EmployeeTimers", "ApprovedBySupervisorEmployeeID", "dbo.Employees", "EmployeeID");
            AddForeignKey("dbo.EmployeeTimers", "ApprovedByBillingEmployeeID", "dbo.Employees", "EmployeeID");
            AddForeignKey("dbo.EmployeeTimers", "ApprovedByAccountingEmployeeID", "dbo.Employees", "EmployeeID");
        }
    }
}
