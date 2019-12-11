namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuthActivityKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuthActivities", "Key", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuthActivities", "Key");
        }
    }
}
