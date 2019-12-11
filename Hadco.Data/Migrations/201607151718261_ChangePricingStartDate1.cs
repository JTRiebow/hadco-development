namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePricingStartDate1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Pricings", "StartDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.Pricings", "EndDate", c => c.DateTime(storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Pricings", "EndDate", c => c.DateTime());
            AlterColumn("dbo.Pricings", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
