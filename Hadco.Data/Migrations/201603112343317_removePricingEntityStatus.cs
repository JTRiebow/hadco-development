namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removePricingEntityStatus : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Pricings", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pricings", "Status", c => c.Int(nullable: false));
        }
    }
}
