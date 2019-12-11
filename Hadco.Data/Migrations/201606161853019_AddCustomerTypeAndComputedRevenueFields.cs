namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerTypeAndComputedRevenueFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "CustomerTypeID", c => c.Int());
            AddColumn("dbo.LoadTimers", "CalculatedRevenue", c => c.Decimal(precision: 8, scale: 2));
            CreateIndex("dbo.Jobs", "CustomerTypeID");
            AddForeignKey("dbo.Jobs", "CustomerTypeID", "dbo.CustomerTypes", "CustomerTypeID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Jobs", "CustomerTypeID", "dbo.CustomerTypes");
            DropIndex("dbo.Jobs", new[] { "CustomerTypeID" });
            DropColumn("dbo.LoadTimers", "CalculatedRevenue");
            DropColumn("dbo.Jobs", "CustomerTypeID");
        }
    }
}
