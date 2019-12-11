namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUpdatedDateToPricing : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pricings", "UpdatedTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            //AddColumn("dbo.TruckerDailies", "PricePerUnit", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            //DropColumn("dbo.TruckerDailies", "PricePerUnit");
            DropColumn("dbo.Pricings", "UpdatedTime");
        }
    }
}
