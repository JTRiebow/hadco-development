namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedTransportLoadTimers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TransportLoadTimers", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.DowntimeTimers", "TransportLoadTimer_ID", "dbo.TransportLoadTimers");
            DropForeignKey("dbo.TransportLoadTimers", "JobID", "dbo.Jobs");
            DropForeignKey("dbo.TransportLoadTimers", "MaterialID", "dbo.Materials");
            DropForeignKey("dbo.TransportLoadTimers", "PhaseID", "dbo.Phases");
            DropForeignKey("dbo.TransportLoadTimers", "TimesheetID", "dbo.Timesheets");
            DropForeignKey("dbo.TransportLoadTimers", "TrailerID", "dbo.Equipment");
            DropForeignKey("dbo.TransportLoadTimers", "TruckID", "dbo.Equipment");
            DropIndex("dbo.DowntimeTimers", new[] { "TransportLoadTimer_ID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "TimesheetID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "TruckID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "TrailerID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "JobID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "PhaseID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "CategoryID" });
            DropIndex("dbo.TransportLoadTimers", new[] { "MaterialID" });
            AddColumn("dbo.LoadTimers", "LoadEquipmentID", c => c.Int());
            CreateIndex("dbo.LoadTimers", "LoadEquipmentID");
            AddForeignKey("dbo.LoadTimers", "LoadEquipmentID", "dbo.Equipment", "EquipmentID");
            DropColumn("dbo.DowntimeTimers", "TransportLoadTimer_ID");
            DropTable("dbo.TransportLoadTimers");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.TransportLoadTimerID);
            
            AddColumn("dbo.DowntimeTimers", "TransportLoadTimer_ID", c => c.Int());
            DropForeignKey("dbo.LoadTimers", "LoadEquipmentID", "dbo.Equipment");
            DropIndex("dbo.LoadTimers", new[] { "LoadEquipmentID" });
            DropColumn("dbo.LoadTimers", "LoadEquipmentID");
            CreateIndex("dbo.TransportLoadTimers", "MaterialID");
            CreateIndex("dbo.TransportLoadTimers", "CategoryID");
            CreateIndex("dbo.TransportLoadTimers", "PhaseID");
            CreateIndex("dbo.TransportLoadTimers", "JobID");
            CreateIndex("dbo.TransportLoadTimers", "TrailerID");
            CreateIndex("dbo.TransportLoadTimers", "TruckID");
            CreateIndex("dbo.TransportLoadTimers", "TimesheetID");
            CreateIndex("dbo.DowntimeTimers", "TransportLoadTimer_ID");
            AddForeignKey("dbo.TransportLoadTimers", "TruckID", "dbo.Equipment", "EquipmentID");
            AddForeignKey("dbo.TransportLoadTimers", "TrailerID", "dbo.Equipment", "EquipmentID");
            AddForeignKey("dbo.TransportLoadTimers", "TimesheetID", "dbo.Timesheets", "TimesheetID", cascadeDelete: true);
            AddForeignKey("dbo.TransportLoadTimers", "PhaseID", "dbo.Phases", "PhaseID");
            AddForeignKey("dbo.TransportLoadTimers", "MaterialID", "dbo.Materials", "MaterialID");
            AddForeignKey("dbo.TransportLoadTimers", "JobID", "dbo.Jobs", "JobID");
            AddForeignKey("dbo.DowntimeTimers", "TransportLoadTimer_ID", "dbo.TransportLoadTimers", "TransportLoadTimerID");
            AddForeignKey("dbo.TransportLoadTimers", "CategoryID", "dbo.Categories", "CategoryID");
        }
    }
}
