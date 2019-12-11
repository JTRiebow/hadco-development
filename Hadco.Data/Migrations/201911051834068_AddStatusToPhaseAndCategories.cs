namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusToPhaseAndCategories : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "Status", c => c.Int(nullable: false, defaultValue:1));
            AddColumn("dbo.Phases", "Status", c => c.Int(nullable: false, defaultValue: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Phases", "Status");
            DropColumn("dbo.Categories", "Status");
        }
    }
}
