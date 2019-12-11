namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedTimerTypes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Locations", "LoadTimerID", "dbo.LoadTimers");
            DropForeignKey("dbo.Locations", "TimerEntryTypeID", "dbo.TimerEntryTypes");
            DropIndex("dbo.Locations", new[] { "LoadTimerID" });
            DropIndex("dbo.Locations", new[] { "TimerEntryTypeID" });
            DropColumn("dbo.Locations", "LoadTimerID");
            DropColumn("dbo.Locations", "IsOutsideGeofence");
            DropColumn("dbo.Locations", "TimerEntryTypeID");
            DropTable("dbo.TimerEntryTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TimerEntryTypes",
                c => new
                    {
                        TimerEntryTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 32),
                    })
                .PrimaryKey(t => t.TimerEntryTypeID);
            
            AddColumn("dbo.Locations", "TimerEntryTypeID", c => c.Int());
            AddColumn("dbo.Locations", "IsOutsideGeofence", c => c.Boolean(nullable: false));
            AddColumn("dbo.Locations", "LoadTimerID", c => c.Int());
            CreateIndex("dbo.Locations", "TimerEntryTypeID");
            CreateIndex("dbo.Locations", "LoadTimerID");
            AddForeignKey("dbo.Locations", "TimerEntryTypeID", "dbo.TimerEntryTypes", "TimerEntryTypeID");
            AddForeignKey("dbo.Locations", "LoadTimerID", "dbo.LoadTimers", "LoadTimerID");
        }
    }
}
