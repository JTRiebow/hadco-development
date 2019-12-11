namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeLoadTimerEntryNames : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.LoadTimerEntries", "LoadTime", "StartTime");
            RenameColumn("dbo.LoadTimerEntries", "LoadTimeLatitude", "StartTimeLatitude");
            RenameColumn("dbo.LoadTimerEntries", "LoadTimeLongitude", "StartTimeLongitude");
            RenameColumn("dbo.LoadTimerEntries", "DumpTime", "EndTime");
            RenameColumn("dbo.LoadTimerEntries", "DumpTimeLatitude", "EndTimeLatitude");
            RenameColumn("dbo.LoadTimerEntries", "DumpTimeLongitude", "EndTimeLongitude");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.LoadTimerEntries", "StartTime", "LoadTime");
            RenameColumn("dbo.LoadTimerEntries", "StartTimeLatitude", "LoadTimeLatitude");
            RenameColumn("dbo.LoadTimerEntries", "StartTimeLongitude", "LoadTimeLongitude");
            RenameColumn("dbo.LoadTimerEntries", "DumpTime", "EndTime");
            RenameColumn("dbo.LoadTimerEntries", "EndTimeLatitude", "DumpTimeLatitude");
            RenameColumn("dbo.LoadTimerEntries", "EndTimeLongitude", "DumpTimeLongitude");
        }
    }
}
