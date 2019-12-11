namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBreakTimer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BreakTimers",
                c => new
                    {
                        BreakTimerID = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTimeOffset(precision: 7),
                        StopTime = c.DateTimeOffset(precision: 7),
                        LoadTimerID = c.Int(),
                    })
                .PrimaryKey(t => t.BreakTimerID)
                .ForeignKey("dbo.LoadTimers", t => t.LoadTimerID)
                .Index(t => t.LoadTimerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BreakTimers", "LoadTimerID", "dbo.LoadTimers");
            DropIndex("dbo.BreakTimers", new[] { "LoadTimerID" });
            DropTable("dbo.BreakTimers");
        }
    }
}
