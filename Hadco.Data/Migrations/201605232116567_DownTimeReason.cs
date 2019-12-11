namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DownTimeReason : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DowntimeReasons", "UseLoadPhase", c => c.Boolean(nullable: false));
            AddColumn("dbo.DowntimeReasons", "UseLoadCategory", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DowntimeReasons", "UseLoadCategory");
            DropColumn("dbo.DowntimeReasons", "UseLoadPhase");
        }
    }
}
