namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePricing : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Pricings");
            AlterColumn("dbo.Pricings", "PricingID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Pricings", "PricingID");
            CreateIndex("dbo.Pricings", "CustomerTypeID");
            CreateIndex("dbo.Pricings", "BillTypeID");
            CreateIndex("dbo.Pricings", "RunID");
            CreateIndex("dbo.Pricings", "JobID");
            CreateIndex("dbo.Pricings", "CustomerID");
            AddForeignKey("dbo.Pricings", "BillTypeID", "dbo.BillTypes", "BillTypeID", cascadeDelete: true);
            AddForeignKey("dbo.Pricings", "CustomerID", "dbo.Customers", "CustomerID");
            AddForeignKey("dbo.Pricings", "CustomerTypeID", "dbo.CustomerTypes", "CustomerTypeID", cascadeDelete: true);
            AddForeignKey("dbo.Pricings", "JobID", "dbo.Jobs", "JobID");
            AddForeignKey("dbo.Pricings", "RunID", "dbo.Phases", "PhaseID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pricings", "RunID", "dbo.Phases");
            DropForeignKey("dbo.Pricings", "JobID", "dbo.Jobs");
            DropForeignKey("dbo.Pricings", "CustomerTypeID", "dbo.CustomerTypes");
            DropForeignKey("dbo.Pricings", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Pricings", "BillTypeID", "dbo.BillTypes");
            DropIndex("dbo.Pricings", new[] { "CustomerID" });
            DropIndex("dbo.Pricings", new[] { "JobID" });
            DropIndex("dbo.Pricings", new[] { "RunID" });
            DropIndex("dbo.Pricings", new[] { "BillTypeID" });
            DropIndex("dbo.Pricings", new[] { "CustomerTypeID" });
            DropPrimaryKey("dbo.Pricings");
            AlterColumn("dbo.Pricings", "PricingID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Pricings", "PricingID");
        }
    }
}
