namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTruckerDailyView : DbMigration
    {
        public override void Up()
        {
            Sql(@"IF OBJECT_ID('TruckerDailies', 'view') IS NOT NULL
                                DROP VIEW TruckerDailies;");
            Sql(Data.Resources.CreateTruckerDailyView);
        }
        
        public override void Down()
        {
        }
    }
}
