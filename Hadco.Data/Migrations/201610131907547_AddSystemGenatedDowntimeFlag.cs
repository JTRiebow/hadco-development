namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSystemGenatedDowntimeFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DowntimeTimers", "SystemGenerated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DowntimeTimers", "SystemGenerated");
        }
    }
}
