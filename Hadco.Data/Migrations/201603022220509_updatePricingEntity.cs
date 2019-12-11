namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePricingEntity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Prices", "Value", c => c.Decimal(precision: 18, scale: 2));
            CreateIndex("dbo.Prices", "PricingID");
            AddForeignKey("dbo.Prices", "PricingID", "dbo.Pricings", "PricingID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Prices", "PricingID", "dbo.Pricings");
            DropIndex("dbo.Prices", new[] { "PricingID" });
            AlterColumn("dbo.Prices", "Value", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
