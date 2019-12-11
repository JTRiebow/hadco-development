namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlannedQuantities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.JobTimers", "UnitID", "dbo.Units");
            DropIndex("dbo.JobTimers", new[] { "UnitID" });
            AddColumn("dbo.Categories", "UnitsOfMeasure", c => c.String());
            AddColumn("dbo.Categories", "PlannedQuantity", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.JobTimers", "NewQuantity", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.JobTimers", "Quantity");
            DropColumn("dbo.JobTimers", "PlanQuantity");
            DropColumn("dbo.JobTimers", "UnitID");
            DropTable("dbo.Units");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Units",
                c => new
                    {
                        UnitID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 32),
                    })
                .PrimaryKey(t => t.UnitID);
            
            AddColumn("dbo.JobTimers", "UnitID", c => c.Int());
            AddColumn("dbo.JobTimers", "PlanQuantity", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.JobTimers", "Quantity", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.JobTimers", "NewQuantity");
            DropColumn("dbo.Categories", "PlannedQuantity");
            DropColumn("dbo.Categories", "UnitsOfMeasure");
            CreateIndex("dbo.JobTimers", "UnitID");
            AddForeignKey("dbo.JobTimers", "UnitID", "dbo.Units", "UnitID");
        }
    }
}
