namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFKtoTimesheetOnJobTimer : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.JobTimers", "Timesheet_ID", "dbo.Timesheets");
            DropIndex("dbo.JobTimers", new[] { "Timesheet_ID" });
            RenameColumn(table: "dbo.JobTimers", name: "Timesheet_ID", newName: "TimesheetID");
            AlterColumn("dbo.JobTimers", "TimesheetID", c => c.Int(nullable: false));
            CreateIndex("dbo.JobTimers", "TimesheetID");
            AddForeignKey("dbo.JobTimers", "TimesheetID", "dbo.Timesheets", "TimesheetID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobTimers", "TimesheetID", "dbo.Timesheets");
            DropIndex("dbo.JobTimers", new[] { "TimesheetID" });
            AlterColumn("dbo.JobTimers", "TimesheetID", c => c.Int());
            RenameColumn(table: "dbo.JobTimers", name: "TimesheetID", newName: "Timesheet_ID");
            CreateIndex("dbo.JobTimers", "Timesheet_ID");
            AddForeignKey("dbo.JobTimers", "Timesheet_ID", "dbo.Timesheets", "TimesheetID");
        }
    }
}
