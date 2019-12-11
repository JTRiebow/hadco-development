namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jobidAddedToCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "JobID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "JobID");
        }
    }
}
