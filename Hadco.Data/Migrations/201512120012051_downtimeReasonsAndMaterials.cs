namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class downtimeReasonsAndMaterials : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DowntimeReasons",
                c => new
                    {
                        DowntimeReasonID = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 128),
                        Code = c.String(maxLength: 16),
                        JobNumber = c.String(maxLength: 128),
                        PhaseNumber = c.String(maxLength: 128),
                        CategoryNumber = c.String(maxLength: 128),
                        UseLoadJob = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DowntimeReasonID);
            
            CreateTable(
                "dbo.Materials",
                c => new
                    {
                        MaterialID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 128),
                        CategoryNumber = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MaterialID);
            
            AddColumn("dbo.LoadTimers", "MaterialID", c => c.Int(nullable: false));
            AddColumn("dbo.LoadTimers", "DowntimeReasonID", c => c.Int(nullable: false));
            CreateIndex("dbo.LoadTimers", "MaterialID");
            CreateIndex("dbo.LoadTimers", "DowntimeReasonID");
            AddForeignKey("dbo.LoadTimers", "DowntimeReasonID", "dbo.DowntimeReasons", "DowntimeReasonID", cascadeDelete: false);
            AddForeignKey("dbo.LoadTimers", "MaterialID", "dbo.Materials", "MaterialID", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LoadTimers", "MaterialID", "dbo.Materials");
            DropForeignKey("dbo.LoadTimers", "DowntimeReasonID", "dbo.DowntimeReasons");
            DropIndex("dbo.LoadTimers", new[] { "DowntimeReasonID" });
            DropIndex("dbo.LoadTimers", new[] { "MaterialID" });
            DropColumn("dbo.LoadTimers", "DowntimeReasonID");
            DropColumn("dbo.LoadTimers", "MaterialID");
            DropTable("dbo.Materials");
            DropTable("dbo.DowntimeReasons");
        }
    }
}
