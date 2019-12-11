namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsDumpedToLoadTimer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoadTimers", "IsDumped", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoadTimers", "IsDumped");
        }
    }
}
