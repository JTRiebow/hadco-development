namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPricesAndPricing : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        PriceID = c.Int(nullable: false, identity: true),
                        PricingID = c.Int(nullable: false),
                        MaterialID = c.Int(),
                        TruckClassificationID = c.Int(),
                        PriceValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PriceID);
            
            CreateTable(
                "dbo.Pricings",
                c => new
                    {
                        PricingID = c.Int(nullable: false, identity: true),
                        CustomerTypeID = c.Int(nullable: false),
                        BillTypeID = c.Int(nullable: false),
                        RunID = c.Int(),
                        JobID = c.Int(),
                        CustomerID = c.Int(),
                        StartTime = c.DateTimeOffset(precision: 7),
                        EndTime = c.DateTimeOffset(precision: 7),
                    })
                .PrimaryKey(t => t.PricingID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Pricings");
            DropTable("dbo.Prices");
        }
    }
}
