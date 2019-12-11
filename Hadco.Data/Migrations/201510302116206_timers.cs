namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeeTimers",
                c => new
                    {
                        EmployeeTimerID = c.Int(nullable: false, identity: true),
                        TimesheetID = c.Int(nullable: false),
                        StartTime = c.DateTimeOffset(nullable: false, precision: 7),
                        LunchStart = c.DateTimeOffset(nullable: false, precision: 7),
                        LunchStop = c.DateTimeOffset(nullable: false, precision: 7),
                        StopTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Injured = c.Boolean(nullable: false),
                        EquipmentID = c.Int(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                        ShopMinutes = c.Int(nullable: false),
                        TravelMinutes = c.Int(nullable: false),
                        GreaseMinutes = c.Int(nullable: false),
                        DailyMinutes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeTimerID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: false)
                .ForeignKey("dbo.Equipment", t => t.EquipmentID, cascadeDelete: false)
                .ForeignKey("dbo.Timesheets", t => t.TimesheetID, cascadeDelete: true)
                .Index(t => t.TimesheetID)
                .Index(t => t.EquipmentID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.Occurrences",
                c => new
                    {
                        OccurrenceID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 32),
                    })
                .PrimaryKey(t => t.OccurrenceID);
            
            CreateTable(
                "dbo.Timesheets",
                c => new
                    {
                        TimesheetID = c.Int(nullable: false, identity: true),
                        EquipmentID = c.Int(nullable: false),
                        EquipmentUseTime = c.Int(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                        Day = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TimesheetID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: false)
                .ForeignKey("dbo.Equipment", t => t.EquipmentID, cascadeDelete: false)
                .Index(t => t.EquipmentID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.JobTimers",
                c => new
                    {
                        JobTimerID = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTimeOffset(nullable: false, precision: 7),
                        StopTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Diary = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitID = c.Int(nullable: false),
                        JobID = c.Int(nullable: false),
                        PhaseID = c.Int(nullable: false),
                        CategoryID = c.Int(nullable: false),
                        Timesheet_ID = c.Int(),
                    })
                .PrimaryKey(t => t.JobTimerID)
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: false)
                .ForeignKey("dbo.Jobs", t => t.JobID, cascadeDelete: false)
                .ForeignKey("dbo.Phases", t => t.PhaseID, cascadeDelete: false)
                .ForeignKey("dbo.Units", t => t.UnitID, cascadeDelete: false)
                .ForeignKey("dbo.Timesheets", t => t.Timesheet_ID)
                .Index(t => t.UnitID)
                .Index(t => t.JobID)
                .Index(t => t.PhaseID)
                .Index(t => t.CategoryID)
                .Index(t => t.Timesheet_ID);
            
            CreateTable(
                "dbo.EmployeeJobTimers",
                c => new
                    {
                        EmployeeJobTimerID = c.Int(nullable: false, identity: true),
                        JobTimerID = c.Int(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                        EquipmentMinutes = c.Int(nullable: false),
                        LaborMinutes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeJobTimerID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: false)
                .ForeignKey("dbo.JobTimers", t => t.JobTimerID, cascadeDelete: true)
                .Index(t => t.JobTimerID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.Units",
                c => new
                    {
                        UnitID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 32),
                    })
                .PrimaryKey(t => t.UnitID);
            
            CreateTable(
                "dbo.EmployeTimerOccurrences",
                c => new
                    {
                        EmployeeTimerID = c.Int(nullable: false),
                        OccurrenceID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EmployeeTimerID, t.OccurrenceID })
                .ForeignKey("dbo.EmployeeTimers", t => t.EmployeeTimerID, cascadeDelete: true)
                .ForeignKey("dbo.Occurrences", t => t.OccurrenceID, cascadeDelete: false)
                .Index(t => t.EmployeeTimerID)
                .Index(t => t.OccurrenceID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobTimers", "Timesheet_ID", "dbo.Timesheets");
            DropForeignKey("dbo.JobTimers", "UnitID", "dbo.Units");
            DropForeignKey("dbo.JobTimers", "PhaseID", "dbo.Phases");
            DropForeignKey("dbo.JobTimers", "JobID", "dbo.Jobs");
            DropForeignKey("dbo.EmployeeJobTimers", "JobTimerID", "dbo.JobTimers");
            DropForeignKey("dbo.EmployeeJobTimers", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.JobTimers", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.Timesheets", "EquipmentID", "dbo.Equipment");
            DropForeignKey("dbo.EmployeeTimers", "TimesheetID", "dbo.Timesheets");
            DropForeignKey("dbo.Timesheets", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.EmployeTimerOccurrences", "OccurrenceID", "dbo.Occurrences");
            DropForeignKey("dbo.EmployeTimerOccurrences", "EmployeeTimerID", "dbo.EmployeeTimers");
            DropForeignKey("dbo.EmployeeTimers", "EquipmentID", "dbo.Equipment");
            DropForeignKey("dbo.EmployeeTimers", "EmployeeID", "dbo.Employees");
            DropIndex("dbo.EmployeTimerOccurrences", new[] { "OccurrenceID" });
            DropIndex("dbo.EmployeTimerOccurrences", new[] { "EmployeeTimerID" });
            DropIndex("dbo.EmployeeJobTimers", new[] { "EmployeeID" });
            DropIndex("dbo.EmployeeJobTimers", new[] { "JobTimerID" });
            DropIndex("dbo.JobTimers", new[] { "Timesheet_ID" });
            DropIndex("dbo.JobTimers", new[] { "CategoryID" });
            DropIndex("dbo.JobTimers", new[] { "PhaseID" });
            DropIndex("dbo.JobTimers", new[] { "JobID" });
            DropIndex("dbo.JobTimers", new[] { "UnitID" });
            DropIndex("dbo.Timesheets", new[] { "EmployeeID" });
            DropIndex("dbo.Timesheets", new[] { "EquipmentID" });
            DropIndex("dbo.EmployeeTimers", new[] { "EmployeeID" });
            DropIndex("dbo.EmployeeTimers", new[] { "EquipmentID" });
            DropIndex("dbo.EmployeeTimers", new[] { "TimesheetID" });
            DropTable("dbo.EmployeTimerOccurrences");
            DropTable("dbo.Units");
            DropTable("dbo.EmployeeJobTimers");
            DropTable("dbo.JobTimers");
            DropTable("dbo.Timesheets");
            DropTable("dbo.Occurrences");
            DropTable("dbo.EmployeeTimers");
        }
    }
}
