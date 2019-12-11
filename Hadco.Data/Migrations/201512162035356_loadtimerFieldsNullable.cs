namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loadtimerFieldsNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LoadTimers", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.LoadTimers", "JobID", "dbo.Jobs");
            DropForeignKey("dbo.LoadTimers", "MaterialID", "dbo.Materials");
            DropForeignKey("dbo.LoadTimers", "PhaseID", "dbo.Phases");
            DropForeignKey("dbo.LoadTimers", "TrailerID", "dbo.Equipment");
            DropForeignKey("dbo.LoadTimers", "TruckID", "dbo.Equipment");
            DropIndex("dbo.LoadTimers", new[] { "TruckID" });
            DropIndex("dbo.LoadTimers", new[] { "TrailerID" });
            DropIndex("dbo.LoadTimers", new[] { "JobID" });
            DropIndex("dbo.LoadTimers", new[] { "PhaseID" });
            DropIndex("dbo.LoadTimers", new[] { "CategoryID" });
            DropIndex("dbo.LoadTimers", new[] { "MaterialID" });
            AddColumn("dbo.DowntimeTimers", "StartTimeLatitude", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.DowntimeTimers", "StartTimeLongitude", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.DowntimeTimers", "StopTimeLatitude", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.DowntimeTimers", "StopTimeLongitude", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.LoadTimers", "TruckID", c => c.Int());
            AlterColumn("dbo.LoadTimers", "TrailerID", c => c.Int());
            AlterColumn("dbo.LoadTimers", "JobID", c => c.Int());
            AlterColumn("dbo.LoadTimers", "PhaseID", c => c.Int());
            AlterColumn("dbo.LoadTimers", "CategoryID", c => c.Int());
            AlterColumn("dbo.LoadTimers", "Tons", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.LoadTimers", "MaterialID", c => c.Int());
            CreateIndex("dbo.LoadTimers", "TruckID");
            CreateIndex("dbo.LoadTimers", "TrailerID");
            CreateIndex("dbo.LoadTimers", "JobID");
            CreateIndex("dbo.LoadTimers", "PhaseID");
            CreateIndex("dbo.LoadTimers", "CategoryID");
            CreateIndex("dbo.LoadTimers", "MaterialID");
            AddForeignKey("dbo.LoadTimers", "CategoryID", "dbo.Categories", "CategoryID");
            AddForeignKey("dbo.LoadTimers", "JobID", "dbo.Jobs", "JobID");
            AddForeignKey("dbo.LoadTimers", "MaterialID", "dbo.Materials", "MaterialID");
            AddForeignKey("dbo.LoadTimers", "PhaseID", "dbo.Phases", "PhaseID");
            AddForeignKey("dbo.LoadTimers", "TrailerID", "dbo.Equipment", "EquipmentID");
            AddForeignKey("dbo.LoadTimers", "TruckID", "dbo.Equipment", "EquipmentID");
        }
        
        public override void Down()
        {
            
        }
    }
}
