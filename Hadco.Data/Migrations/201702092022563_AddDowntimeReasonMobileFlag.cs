namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDowntimeReasonMobileFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DowntimeReasons", "DisplayOnMobile", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DowntimeReasons", "DisplayOnMobile");
        }
    }
}
