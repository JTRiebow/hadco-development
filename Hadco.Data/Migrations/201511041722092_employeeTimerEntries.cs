namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeeTimerEntries : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmployeeTimers", "TimesheetID", "dbo.Timesheets");
            DropIndex("dbo.EmployeeTimers", new[] { "TimesheetID" });
            CreateTable(
                "dbo.EmployeeTimerEntries",
                c => new
                    {
                        EmployeeTimerEntryID = c.Int(nullable: false, identity: true),
                        EmployeeTimerID = c.Int(nullable: false),
                        ClockIn = c.DateTimeOffset(nullable: false, precision: 7),
                        ClockOut = c.DateTimeOffset(precision: 7),
                    })
                .PrimaryKey(t => t.EmployeeTimerEntryID)
                .ForeignKey("dbo.EmployeeTimers", t => t.EmployeeTimerID, cascadeDelete: true)
                .Index(t => t.EmployeeTimerID);
            
            AddColumn("dbo.EmployeeTimers", "Day", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.EmployeeTimers", "TimesheetID", c => c.Int());
            CreateIndex("dbo.EmployeeTimers", "TimesheetID");
            AddForeignKey("dbo.EmployeeTimers", "TimesheetID", "dbo.Timesheets", "TimesheetID");
            DropColumn("dbo.EmployeeTimers", "StartTime");
            DropColumn("dbo.EmployeeTimers", "LunchStart");
            DropColumn("dbo.EmployeeTimers", "LunchStop");
            DropColumn("dbo.EmployeeTimers", "StopTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmployeeTimers", "StopTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EmployeeTimers", "LunchStop", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EmployeeTimers", "LunchStart", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EmployeeTimers", "StartTime", c => c.DateTimeOffset(precision: 7));
            DropForeignKey("dbo.EmployeeTimers", "TimesheetID", "dbo.Timesheets");
            DropForeignKey("dbo.EmployeeTimerEntries", "EmployeeTimerID", "dbo.EmployeeTimers");
            DropIndex("dbo.EmployeeTimerEntries", new[] { "EmployeeTimerID" });
            DropIndex("dbo.EmployeeTimers", new[] { "TimesheetID" });
            //AlterColumn("dbo.EmployeeTimers", "TimesheetID", c => c.Int(nullable: false));
            DropColumn("dbo.EmployeeTimers", "Day");
            DropTable("dbo.EmployeeTimerEntries");
            CreateIndex("dbo.EmployeeTimers", "TimesheetID");
            AddForeignKey("dbo.EmployeeTimers", "TimesheetID", "dbo.Timesheets", "TimesheetID", cascadeDelete: true);
        }
    }
}
