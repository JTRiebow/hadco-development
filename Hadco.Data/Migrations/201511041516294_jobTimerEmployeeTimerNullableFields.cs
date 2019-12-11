namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jobTimerEmployeeTimerNullableFields : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmployeeTimers", "EquipmentID", "dbo.Equipment");
            DropForeignKey("dbo.JobTimers", "UnitID", "dbo.Units");
            DropIndex("dbo.EmployeeTimers", new[] { "EquipmentID" });
            DropIndex("dbo.JobTimers", new[] { "UnitID" });
            AlterColumn("dbo.EmployeeTimers", "StartTime", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.EmployeeTimers", "LunchStart", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.EmployeeTimers", "LunchStop", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.EmployeeTimers", "StopTime", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.EmployeeTimers", "EquipmentID", c => c.Int());
            AlterColumn("dbo.EmployeeTimers", "ShopMinutes", c => c.Int());
            AlterColumn("dbo.EmployeeTimers", "TravelMinutes", c => c.Int());
            AlterColumn("dbo.EmployeeTimers", "GreaseMinutes", c => c.Int());
            AlterColumn("dbo.EmployeeTimers", "DailyMinutes", c => c.Int());
            AlterColumn("dbo.JobTimers", "Quantity", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.JobTimers", "PlanQuantity", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.JobTimers", "UnitID", c => c.Int());
            CreateIndex("dbo.EmployeeTimers", "EquipmentID");
            CreateIndex("dbo.JobTimers", "UnitID");
            AddForeignKey("dbo.EmployeeTimers", "EquipmentID", "dbo.Equipment", "EquipmentID");
            AddForeignKey("dbo.JobTimers", "UnitID", "dbo.Units", "UnitID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobTimers", "UnitID", "dbo.Units");
            DropForeignKey("dbo.EmployeeTimers", "EquipmentID", "dbo.Equipment");
            DropIndex("dbo.JobTimers", new[] { "UnitID" });
            DropIndex("dbo.EmployeeTimers", new[] { "EquipmentID" });
            //AlterColumn("dbo.JobTimers", "UnitID", c => c.Int(nullable: false));
            //AlterColumn("dbo.JobTimers", "PlanQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AlterColumn("dbo.JobTimers", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AlterColumn("dbo.EmployeeTimers", "DailyMinutes", c => c.Int(nullable: false));
            //AlterColumn("dbo.EmployeeTimers", "GreaseMinutes", c => c.Int(nullable: false));
            //AlterColumn("dbo.EmployeeTimers", "TravelMinutes", c => c.Int(nullable: false));
            //AlterColumn("dbo.EmployeeTimers", "ShopMinutes", c => c.Int(nullable: false));
            //AlterColumn("dbo.EmployeeTimers", "EquipmentID", c => c.Int(nullable: false));
            //AlterColumn("dbo.EmployeeTimers", "StopTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            //AlterColumn("dbo.EmployeeTimers", "LunchStop", c => c.DateTimeOffset(nullable: false, precision: 7));
            //AlterColumn("dbo.EmployeeTimers", "LunchStart", c => c.DateTimeOffset(nullable: false, precision: 7));
            //AlterColumn("dbo.EmployeeTimers", "StartTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            CreateIndex("dbo.JobTimers", "UnitID");
            CreateIndex("dbo.EmployeeTimers", "EquipmentID");
            AddForeignKey("dbo.JobTimers", "UnitID", "dbo.Units", "UnitID", cascadeDelete: true);
            AddForeignKey("dbo.EmployeeTimers", "EquipmentID", "dbo.Equipment", "EquipmentID", cascadeDelete: true);
        }
    }
}
