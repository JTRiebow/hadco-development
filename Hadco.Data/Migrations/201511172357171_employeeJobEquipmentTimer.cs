namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeeJobEquipmentTimer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeeJobEquipmentTimers",
                c => new
                    {
                        EmployeeJobEquipmentTimerID = c.Int(nullable: false, identity: true),
                        EmployeeJobTimerID = c.Int(nullable: false),
                        EquipmentID = c.Int(nullable: false),
                        EquipmentMinutes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeJobEquipmentTimerID)
                .ForeignKey("dbo.EmployeeJobTimers", t => t.EmployeeJobTimerID, cascadeDelete: true)
                .ForeignKey("dbo.Equipment", t => t.EquipmentID, cascadeDelete: true)
                .Index(t => t.EmployeeJobTimerID)
                .Index(t => t.EquipmentID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeJobEquipmentTimers", "EquipmentID", "dbo.Equipment");
            DropForeignKey("dbo.EmployeeJobEquipmentTimers", "EmployeeJobTimerID", "dbo.EmployeeJobTimers");
            DropIndex("dbo.EmployeeJobEquipmentTimers", new[] { "EquipmentID" });
            DropIndex("dbo.EmployeeJobEquipmentTimers", new[] { "EmployeeJobTimerID" });
            DropTable("dbo.EmployeeJobEquipmentTimers");
        }
    }
}
