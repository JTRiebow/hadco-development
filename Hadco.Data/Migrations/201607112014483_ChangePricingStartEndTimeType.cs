namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePricingStartEndTimeType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Pricings", "StartTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Pricings", "EndTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Pricings", "EndTime", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Pricings", "StartTime", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
    }
}
