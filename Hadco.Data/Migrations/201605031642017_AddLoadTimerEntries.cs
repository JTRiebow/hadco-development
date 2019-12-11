namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddLoadTimerEntries : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoadTimerEntries",
                c => new
                {
                    LoadTimerEntryID = c.Int(nullable: false, identity: true),
                    LoadTimerID = c.Int(nullable: false),
                    LoadTime = c.DateTimeOffset(precision: 7),
                    DumpTime = c.DateTimeOffset(precision: 7),
                    LoadTimeLatitude = c.Decimal(precision: 9, scale: 6),
                    LoadTimeLongitude = c.Decimal(precision: 9, scale: 6),
                    DumpTimeLatitude = c.Decimal(precision: 9, scale: 6),
                    DumpTimeLongitude = c.Decimal(precision: 9, scale: 6),
                })
                .PrimaryKey(t => t.LoadTimerEntryID)
                .ForeignKey("dbo.LoadTimers", t => t.LoadTimerID, cascadeDelete: true)
                .Index(t => t.LoadTimerID);
            Sql(@"INSERT INTO dbo.LoadTimerEntries
                    (LoadTimerID, LoadTime, LoadTimeLatitude, LoadTimeLongitude, DumpTime, DumpTimeLatitude, DumpTimeLongitude)
	          SELECT LoadTimerID, LoadTime, LoadTimeLatitude, LoadTimeLongitude, DumpTime, DumpTimeLatitude, DumpTimeLongitude 
                    FROM dbo.LoadTimers");

            DropColumn("dbo.LoadTimers", "LoadTime");
            DropColumn("dbo.LoadTimers", "DumpTime");
            DropColumn("dbo.LoadTimers", "LoadTimeLatitude");
            DropColumn("dbo.LoadTimers", "LoadTimeLongitude");
            DropColumn("dbo.LoadTimers", "DumpTimeLatitude");
            DropColumn("dbo.LoadTimers", "DumpTimeLongitude");
        }

        public override void Down()
        {
            AddColumn("dbo.LoadTimers", "DumpTimeLongitude", c => c.Decimal(precision: 9, scale: 6));
            AddColumn("dbo.LoadTimers", "DumpTimeLatitude", c => c.Decimal(precision: 9, scale: 6));
            AddColumn("dbo.LoadTimers", "LoadTimeLongitude", c => c.Decimal(precision: 9, scale: 6));
            AddColumn("dbo.LoadTimers", "LoadTimeLatitude", c => c.Decimal(precision: 9, scale: 6));
            AddColumn("dbo.LoadTimers", "DumpTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.LoadTimers", "LoadTime", c => c.DateTimeOffset(precision: 7));
            DropForeignKey("dbo.LoadTimerEntries", "LoadTimerID", "dbo.LoadTimers");
            DropIndex("dbo.LoadTimerEntries", new[] { "LoadTimerID" });
            Sql(@"INSERT INTO dbo.LoadTimers
                    (LoadTimerID, LoadTime, LoadTimeLatitude, LoadTimeLongitude, DumpTime, DumpTimeLatitude, DumpTimeLongitude)
	          SELECT LoadTimerID, LoadTime, LoadTimeLatitude, LoadTimeLongitude, DumpTime, DumpTimeLatitude, DumpTimeLongitude 
                    FROM dbo.LoadTimerEntries");
            DropTable("dbo.LoadTimerEntries");
        }
    }
}
