namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPricePerUnit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoadTimers", "PricePerUnit", c => c.Decimal(precision: 18, scale: 2));
            //Sql(@"IF OBJECT_ID('TruckerDailies', 'view') IS NOT NULL
            //                    DROP VIEW TruckerDailies;");
            //Sql(Data.Resources.CreateTruckerDailyView);
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoadTimers", "PricePerUnit");
        }
    }
}
