using Hadco.Data;
using System;
using System.Data.Entity.Migrations;
namespace Hadco.Data.Migrations
{

    public partial class AddTruckerDailyID : DbMigration
    {
        public override void Up()
        {
            //Sql(@"IF OBJECT_ID('TruckerDailies', 'view') IS NOT NULL
            //                    DROP VIEW TruckerDailies;");
            //Sql(Data.Resources.CreateTruckerDailyView);
        }
        
        public override void Down()
        {

        }
    }
}
