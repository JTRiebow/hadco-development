namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loadTimerEntities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmployeeJobTimers", "EmployeeID", "dbo.Employees");
            DropIndex("dbo.EmployeeJobTimers", new[] { "EmployeeID" });
            RenameColumn(table: "dbo.EquipmentTimers", name: "UnitID", newName: "EquipmentTimerID");
            RenameColumn(table: "dbo.EquipmentServiceTypes", name: "UnitID", newName: "EquipmentServiceTypeID");
            CreateTable(
                "dbo.LoadTimers",
                c => new
                    {
                        LoadTimerID = c.Int(nullable: false, identity: true),
                        TimesheetID = c.Int(nullable: false),
                        TruckID = c.Int(nullable: false),
                        TrailerID = c.Int(nullable: false),
                        PupID = c.Int(),
                        JobID = c.Int(nullable: false),
                        PhaseID = c.Int(nullable: false),
                        CategoryID = c.Int(nullable: false),
                        StartLocation = c.String(maxLength: 128),
                        EndLocation = c.String(maxLength: 128),
                        TicketNumber = c.String(maxLength: 32),
                        Tons = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LoadTime = c.DateTimeOffset(precision: 7),
                        DumpTime = c.DateTimeOffset(precision: 7),
                        LoadTimeLatitude = c.Decimal(precision: 9, scale: 6),
                        LoadTimeLongitude = c.Decimal(precision: 9, scale: 6),
                        DumpTimeLatitude = c.Decimal(precision: 9, scale: 6),
                        DumpTimeLongitude = c.Decimal(precision: 9, scale: 6),
                    })
                .PrimaryKey(t => t.LoadTimerID)
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: false)
                .ForeignKey("dbo.Jobs", t => t.JobID, cascadeDelete: false)
                .ForeignKey("dbo.Phases", t => t.PhaseID, cascadeDelete: false)
                .ForeignKey("dbo.Equipment", t => t.PupID)
                .ForeignKey("dbo.Timesheets", t => t.TimesheetID, cascadeDelete: true)
                .ForeignKey("dbo.Equipment", t => t.TrailerID, cascadeDelete: false)
                .ForeignKey("dbo.Equipment", t => t.TruckID, cascadeDelete: false)
                .Index(t => t.TimesheetID)
                .Index(t => t.TruckID)
                .Index(t => t.TrailerID)
                .Index(t => t.PupID)
                .Index(t => t.JobID)
                .Index(t => t.PhaseID)
                .Index(t => t.CategoryID);
            
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
                .PrimaryKey(t => t.TimesheetTimerEntryID)
                .ForeignKey("dbo.Timesheets", t => t.TimesheetID, cascadeDelete: true)
                .ForeignKey("dbo.TimesheetEntryTypes", t => t.TimesheetEntryTypeID, cascadeDelete: false)
                .Index(t => t.TimesheetID)
                .Index(t => t.TimesheetEntryTypeID);
            
            CreateTable(
                "dbo.TimesheetEntryTypes",
                c => new
                    {
                        TimesheetEntryTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 32),
                    })
                .PrimaryKey(t => t.TimesheetEntryTypeID);
            
            AddColumn("dbo.EmployeeJobTimers", "EmployeeTimerID", c => c.Int(nullable: false));
            AddColumn("dbo.Timesheets", "Odometer", c => c.Int());
            AddColumn("dbo.Timesheets", "Notes", c => c.String());
            CreateIndex("dbo.EmployeeJobTimers", "EmployeeTimerID");
            AddForeignKey("dbo.EmployeeJobTimers", "EmployeeTimerID", "dbo.EmployeeTimers", "EmployeeTimerID", cascadeDelete: true);
            DropColumn("dbo.EmployeeJobTimers", "EmployeeID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmployeeJobTimers", "EmployeeID", c => c.Int(nullable: false));
            DropForeignKey("dbo.EmployeeJobTimers", "EmployeeTimerID", "dbo.EmployeeTimers");
            DropForeignKey("dbo.TimesheetTimerEntries", "TimesheetEntryTypeID", "dbo.TimesheetEntryTypes");
            DropForeignKey("dbo.TimesheetTimerEntries", "TimesheetID", "dbo.Timesheets");
            DropForeignKey("dbo.LoadTimers", "TruckID", "dbo.Equipment");
            DropForeignKey("dbo.LoadTimers", "TrailerID", "dbo.Equipment");
            DropForeignKey("dbo.LoadTimers", "TimesheetID", "dbo.Timesheets");
            DropForeignKey("dbo.LoadTimers", "PupID", "dbo.Equipment");
            DropForeignKey("dbo.LoadTimers", "PhaseID", "dbo.Phases");
            DropForeignKey("dbo.LoadTimers", "JobID", "dbo.Jobs");
            DropForeignKey("dbo.LoadTimers", "CategoryID", "dbo.Categories");
            DropIndex("dbo.TimesheetTimerEntries", new[] { "TimesheetEntryTypeID" });
            DropIndex("dbo.TimesheetTimerEntries", new[] { "TimesheetID" });
            DropIndex("dbo.LoadTimers", new[] { "CategoryID" });
            DropIndex("dbo.LoadTimers", new[] { "PhaseID" });
            DropIndex("dbo.LoadTimers", new[] { "JobID" });
            DropIndex("dbo.LoadTimers", new[] { "PupID" });
            DropIndex("dbo.LoadTimers", new[] { "TrailerID" });
            DropIndex("dbo.LoadTimers", new[] { "TruckID" });
            DropIndex("dbo.LoadTimers", new[] { "TimesheetID" });
            DropIndex("dbo.EmployeeJobTimers", new[] { "EmployeeTimerID" });
            DropColumn("dbo.Timesheets", "Notes");
            DropColumn("dbo.Timesheets", "Odometer");
            DropColumn("dbo.EmployeeJobTimers", "EmployeeTimerID");
            DropTable("dbo.TimesheetEntryTypes");
            DropTable("dbo.TimesheetTimerEntries");
            DropTable("dbo.LoadTimers");
            RenameColumn(table: "dbo.EquipmentServiceTypes", name: "EquipmentServiceTypeID", newName: "UnitID");
            RenameColumn(table: "dbo.EquipmentTimers", name: "EquipmentTimerID", newName: "UnitID");
            CreateIndex("dbo.EmployeeJobTimers", "EmployeeID");
            AddForeignKey("dbo.EmployeeJobTimers", "EmployeeID", "dbo.Employees", "EmployeeID", cascadeDelete: true);
        }
    }
}
