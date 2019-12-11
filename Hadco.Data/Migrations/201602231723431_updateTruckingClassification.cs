namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTruckingClassification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prices", "Value", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TruckClassifications", "Name", c => c.String(nullable: false, maxLength: 32));
            AddColumn("dbo.TruckClassifications", "Code", c => c.String(maxLength: 16));
            AlterColumn("dbo.Pricings", "StartTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.TruckClassifications", "Truck", c => c.String(nullable: false, maxLength: 16));
            AlterColumn("dbo.TruckClassifications", "Trailer1", c => c.String(maxLength: 16));
            AlterColumn("dbo.TruckClassifications", "Trailer2", c => c.String(maxLength: 16));
            DropColumn("dbo.Prices", "PriceValue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Prices", "PriceValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.TruckClassifications", "Trailer2", c => c.String(maxLength: 8));
            AlterColumn("dbo.TruckClassifications", "Trailer1", c => c.String(maxLength: 8));
            AlterColumn("dbo.TruckClassifications", "Truck", c => c.String(nullable: false, maxLength: 32));
            AlterColumn("dbo.Pricings", "StartTime", c => c.DateTimeOffset(precision: 7));
            DropColumn("dbo.TruckClassifications", "Code");
            DropColumn("dbo.TruckClassifications", "Name");
            DropColumn("dbo.Prices", "Value");
        }
    }
}
