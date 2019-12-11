namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePricing1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Prices", "PricingID", "dbo.Pricings");
            DropPrimaryKey("dbo.Pricings");
            AddColumn("dbo.Pricings", "Status", c => c.Int(nullable: false));
            AlterColumn("dbo.Pricings", "PricingID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Pricings", "PricingID");
            AddForeignKey("dbo.Prices", "PricingID", "dbo.Pricings", "PricingID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Prices", "PricingID", "dbo.Pricings");
            DropPrimaryKey("dbo.Pricings");
            AlterColumn("dbo.Pricings", "PricingID", c => c.Int(nullable: false));
            DropColumn("dbo.Pricings", "Status");
            AddPrimaryKey("dbo.Pricings", "PricingID");
            AddForeignKey("dbo.Prices", "PricingID", "dbo.Pricings", "PricingID", cascadeDelete: true);
        }
    }
}
