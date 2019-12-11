namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class equipmentTimer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EquipmentServiceTypes",
                c => new
                    {
                        UnitID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 32),
                    })
                .PrimaryKey(t => t.UnitID);
            
            CreateTable(
                "dbo.EquipmentTimers",
                c => new
                    {
                        UnitID = c.Int(nullable: false, identity: true),
                        TimesheetID = c.Int(nullable: false),
                        EquipmentServiceTypeID = c.Int(nullable: false),
                        EquipmentID = c.Int(nullable: false),
                        StartTime = c.DateTimeOffset(precision: 7),
                        StopTime = c.DateTimeOffset(precision: 7),
                        Diary = c.String(),
                        Closed = c.Boolean(),
                    })
                .PrimaryKey(t => t.UnitID)
                .ForeignKey("dbo.Equipment", t => t.EquipmentID, cascadeDelete: true)
                .ForeignKey("dbo.EquipmentServiceTypes", t => t.EquipmentServiceTypeID, cascadeDelete: true)
                .ForeignKey("dbo.Timesheets", t => t.TimesheetID, cascadeDelete: true)
                .Index(t => t.TimesheetID)
                .Index(t => t.EquipmentServiceTypeID)
                .Index(t => t.EquipmentID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EquipmentTimers", "TimesheetID", "dbo.Timesheets");
            DropForeignKey("dbo.EquipmentTimers", "EquipmentServiceTypeID", "dbo.EquipmentServiceTypes");
            DropForeignKey("dbo.EquipmentTimers", "EquipmentID", "dbo.Equipment");
            DropIndex("dbo.EquipmentTimers", new[] { "EquipmentID" });
            DropIndex("dbo.EquipmentTimers", new[] { "EquipmentServiceTypeID" });
            DropIndex("dbo.EquipmentTimers", new[] { "TimesheetID" });
            DropTable("dbo.EquipmentTimers");
            DropTable("dbo.EquipmentServiceTypes");
        }
    }
}
