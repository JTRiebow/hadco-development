namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefactoredLocation : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TimesheetEntryTypes", newName: "TimerEntryTypes");
            DropForeignKey("dbo.TimesheetTimerEntries", "TimesheetID", "dbo.Timesheets");
            DropForeignKey("dbo.TimesheetTimerEntries", "TimesheetEntryTypeID", "dbo.TimesheetEntryTypes");
            DropForeignKey("dbo.BreakTimers", "LoadTimerID", "dbo.LoadTimers");
            DropIndex("dbo.BreakTimers", new[] { "LoadTimerID" });
            DropIndex("dbo.TimesheetTimerEntries", new[] { "TimesheetID" });
            DropIndex("dbo.TimesheetTimerEntries", new[] { "TimesheetEntryTypeID" });
            RenameColumn(table: "dbo.TimerEntryTypes", name: "TimesheetEntryTypeID", newName: "TimerEntryTypeID");
            AddColumn("dbo.Locations", "EmployeeTimerID", c => c.Int(nullable: false));
            AddColumn("dbo.Locations", "LoadTimerID", c => c.Int());
            AddColumn("dbo.Locations", "TimerEntryTypeID", c => c.Int());
            AlterColumn("dbo.Locations", "Latitude", c => c.Decimal(nullable: false, precision: 9, scale: 6));
            AlterColumn("dbo.Locations", "Longitude", c => c.Decimal(nullable: false, precision: 9, scale: 6));
            CreateIndex("dbo.Locations", "EmployeeTimerID");
            CreateIndex("dbo.Locations", "LoadTimerID");
            CreateIndex("dbo.Locations", "TimerEntryTypeID");
            AddForeignKey("dbo.Locations", "EmployeeTimerID", "dbo.EmployeeTimers", "EmployeeTimerID", cascadeDelete: true);
            AddForeignKey("dbo.Locations", "LoadTimerID", "dbo.LoadTimers", "LoadTimerID");
            AddForeignKey("dbo.Locations", "TimerEntryTypeID", "dbo.TimerEntryTypes", "TimerEntryTypeID");
            DropColumn("dbo.Locations", "EmployeeTimerEntryID");
            DropTable("dbo.BreakTimers");
            DropTable("dbo.TimesheetTimerEntries");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TimesheetTimerEntries",
                c => new
                    {
                        TimesheetTimerEntryID = c.Int(nullable: false, identity: true),
                        TimesheetID = c.Int(nullable: false),
                        TimesheetEntryTypeID = c.Int(nullable: false),
                        Time = c.DateTimeOffset(precision: 7),
                        TimeLatitude = c.Decimal(precision: 18, scale: 2),
                        TimeLongitude = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.TimesheetTimerEntryID);
            
            CreateTable(
                "dbo.BreakTimers",
                c => new
                    {
                        BreakTimerID = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTimeOffset(precision: 7),
                        StopTime = c.DateTimeOffset(precision: 7),
                        LoadTimerID = c.Int(),
                    })
                .PrimaryKey(t => t.BreakTimerID);
            
            AddColumn("dbo.Locations", "EmployeeTimerEntryID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Locations", "TimerEntryTypeID", "dbo.TimerEntryTypes");
            DropForeignKey("dbo.Locations", "LoadTimerID", "dbo.LoadTimers");
            DropForeignKey("dbo.Locations", "EmployeeTimerID", "dbo.EmployeeTimers");
            DropIndex("dbo.Locations", new[] { "TimerEntryTypeID" });
            DropIndex("dbo.Locations", new[] { "LoadTimerID" });
            DropIndex("dbo.Locations", new[] { "EmployeeTimerID" });
            AlterColumn("dbo.Locations", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Locations", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Locations", "TimerEntryTypeID");
            DropColumn("dbo.Locations", "LoadTimerID");
            DropColumn("dbo.Locations", "EmployeeTimerID");
            RenameColumn(table: "dbo.TimerEntryTypes", name: "TimerEntryTypeID", newName: "TimesheetEntryTypeID");
            CreateIndex("dbo.TimesheetTimerEntries", "TimesheetEntryTypeID");
            CreateIndex("dbo.TimesheetTimerEntries", "TimesheetID");
            CreateIndex("dbo.BreakTimers", "LoadTimerID");
            AddForeignKey("dbo.BreakTimers", "LoadTimerID", "dbo.LoadTimers", "LoadTimerID");
            AddForeignKey("dbo.TimesheetTimerEntries", "TimesheetEntryTypeID", "dbo.TimesheetEntryTypes", "TimesheetEntryTypeID", cascadeDelete: true);
            AddForeignKey("dbo.TimesheetTimerEntries", "TimesheetID", "dbo.Timesheets", "TimesheetID", cascadeDelete: true);
            RenameTable(name: "dbo.TimerEntryTypes", newName: "TimesheetEntryTypes");
        }
    }
}
