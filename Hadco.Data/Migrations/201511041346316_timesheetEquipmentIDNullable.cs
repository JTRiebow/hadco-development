namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timesheetEquipmentIDNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Timesheets", "EquipmentID", "dbo.Equipment");
            DropIndex("dbo.Timesheets", new[] { "EquipmentID" });
            AlterColumn("dbo.Timesheets", "EquipmentID", c => c.Int());
            CreateIndex("dbo.Timesheets", "EquipmentID");
            AddForeignKey("dbo.Timesheets", "EquipmentID", "dbo.Equipment", "EquipmentID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Timesheets", "EquipmentID", "dbo.Equipment");
            DropIndex("dbo.Timesheets", new[] { "EquipmentID" });
            //AlterColumn("dbo.Timesheets", "EquipmentID", c => c.Int(nullable: false));
            CreateIndex("dbo.Timesheets", "EquipmentID");
            AddForeignKey("dbo.Timesheets", "EquipmentID", "dbo.Equipment", "EquipmentID", cascadeDelete: false);
        }
    }
}
