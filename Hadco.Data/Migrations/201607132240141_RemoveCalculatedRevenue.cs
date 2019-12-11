namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCalculatedRevenue : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.TruckerDailies", "Tons", c => c.String());
            DropColumn("dbo.LoadTimers", "CalculatedRevenue");
            Sql(@"IF OBJECT_ID('TruckerDailies', 'view') IS NOT NULL
                                DROP VIEW TruckerDailies;");
            Sql(Data.Resources.CreateTruckerDailyView);
        }
        
        public override void Down()
        {
            AddColumn("dbo.LoadTimers", "CalculatedRevenue", c => c.Decimal(precision: 18, scale: 2));
            //AlterColumn("dbo.TruckerDailies", "Tons", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
