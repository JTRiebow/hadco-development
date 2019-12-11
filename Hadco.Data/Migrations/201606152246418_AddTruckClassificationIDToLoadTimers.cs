namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTruckClassificationIDToLoadTimers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoadTimers", "TruckClassificationID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoadTimers", "TruckClassificationID");
        }
    }
}
