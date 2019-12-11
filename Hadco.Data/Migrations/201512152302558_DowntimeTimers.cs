namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DowntimeTimers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LoadTimers", "DowntimeReasonID", "dbo.DowntimeReasons");
            DropIndex("dbo.LoadTimers", new[] { "DowntimeReasonID" });
            CreateTable(
                "dbo.DowntimeTimers",
                c => new
                    {
                        DowntimeTimerID = c.Int(nullable: false, identity: true),
                        DowntimeReasonID = c.Int(nullable: false),
                        StartTime = c.DateTimeOffset(precision: 7),
                        StopTime = c.DateTimeOffset(precision: 7),
                        LoadTimerID = c.Int(),
                        TimesheetID = c.Int(),
                    })
                .PrimaryKey(t => t.DowntimeTimerID)
                .ForeignKey("dbo.DowntimeReasons", t => t.DowntimeReasonID, cascadeDelete: true)
                .ForeignKey("dbo.LoadTimers", t => t.LoadTimerID)
                .ForeignKey("dbo.Timesheets", t => t.TimesheetID)
                .Index(t => t.DowntimeReasonID)
                .Index(t => t.LoadTimerID)
                .Index(t => t.TimesheetID);
            
            DropColumn("dbo.LoadTimers", "DowntimeReasonID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LoadTimers", "DowntimeReasonID", c => c.Int(nullable: false));
            DropForeignKey("dbo.DowntimeTimers", "TimesheetID", "dbo.Timesheets");
            DropForeignKey("dbo.DowntimeTimers", "LoadTimerID", "dbo.LoadTimers");
            DropForeignKey("dbo.DowntimeTimers", "DowntimeReasonID", "dbo.DowntimeReasons");
            DropIndex("dbo.DowntimeTimers", new[] { "TimesheetID" });
            DropIndex("dbo.DowntimeTimers", new[] { "LoadTimerID" });
            DropIndex("dbo.DowntimeTimers", new[] { "DowntimeReasonID" });
            DropTable("dbo.DowntimeTimers");
            CreateIndex("dbo.LoadTimers", "DowntimeReasonID");
            AddForeignKey("dbo.LoadTimers", "DowntimeReasonID", "dbo.DowntimeReasons", "DowntimeReasonID", cascadeDelete: true);
        }
    }
}
