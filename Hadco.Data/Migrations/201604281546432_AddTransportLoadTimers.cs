namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransportLoadTimers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransportLoadTimers",
                c => new
                    {
                        TransportLoadTimerID = c.Int(nullable: false, identity: true),
                        TimesheetID = c.Int(nullable: false),
                        TruckID = c.Int(),
                        TrailerID = c.Int(),
                        JobID = c.Int(),
                        PhaseID = c.Int(),
                        CategoryID = c.Int(),
                        StartLocation = c.String(maxLength: 128),
                        EndLocation = c.String(maxLength: 128),
                        LoadTime = c.DateTimeOffset(precision: 7),
                        DumpTime = c.DateTimeOffset(precision: 7),
                        LoadTimeLatitude = c.Decimal(precision: 18, scale: 2),
                        LoadTimeLongitude = c.Decimal(precision: 18, scale: 2),
                        DumpTimeLatitude = c.Decimal(precision: 18, scale: 2),
                        DumpTimeLongitude = c.Decimal(precision: 18, scale: 2),
                        MaterialID = c.Int(),
                        InvoiceNumber = c.String(maxLength: 32),
                        Note = c.String(),
                    })
                .PrimaryKey(t => t.TransportLoadTimerID)
                .ForeignKey("dbo.Categories", t => t.CategoryID)
                .ForeignKey("dbo.Jobs", t => t.JobID)
                .ForeignKey("dbo.Materials", t => t.MaterialID)
                .ForeignKey("dbo.Phases", t => t.PhaseID)
                .ForeignKey("dbo.Timesheets", t => t.TimesheetID, cascadeDelete: true)
                .ForeignKey("dbo.Equipment", t => t.TrailerID)
                .ForeignKey("dbo.Equipment", t => t.TruckID)
                .Index(t => t.TimesheetID)
                .Index(t => t.TruckID)
                .Index(t => t.TrailerID)
                .Index(t => t.JobID)
                .Index(t => t.PhaseID)
                .Index(t => t.CategoryID)
                .Index(t => t.MaterialID);
            
            AddColumn("dbo.DowntimeTimers", "TransportLoadTimer_ID", c => c.Int());
            CreateIndex("dbo.DowntimeTimers", "TransportLoadTimer_ID");
            AddForeignKey("dbo.DowntimeTimers", "TransportLoadTimer_ID", "dbo.TransportLoadTimers", "TransportLoadTimerID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransportLoadTimers", "TruckID", "dbo.Equipment");
            DropForeignKey("dbo.TransportLoadTimers", "TrailerID", "dbo.Equipment");
            DropForeignKey("dbo.TransportLoadTimers", "TimesheetID", "dbo.Timesheets");
            DropForeignKey("dbo.TransportLoadTimers", "PhaseID", "dbo.Phases");
            DropForeignKey("dbo.TransportLoadTimers", "MaterialID", "dbo.Materials");
            DropForeignKey("dbo.TransportLoadTimers", "JobID", "dbo.Jobs");
            DropForeignKey("dbo.DowntimeTimers", "TransportLoadTimer_ID", "dbo.TransportLoadTimers");
            DropForeignKey("dbo.TransportLoadTimers", "CategoryID", "dbo.Categories");
            DropIndex("dbo.TransportLoadTimers", new[] { "MaterialID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "CategoryID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "PhaseID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "JobID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "TrailerID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "TruckID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "TimesheetID" });
            DropIndex("dbo.DowntimeTimers", new[] { "TransportLoadTimer_ID" });
            DropColumn("dbo.DowntimeTimers", "TransportLoadTimer_ID");
            DropTable("dbo.TransportLoadTimers");
        }
    }
}
