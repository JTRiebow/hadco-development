namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dropWeeklySummaryStoredProc : DbMigration
    {
        public override void Up()
        {
            Sql("Drop procedure [dbo].[GetWeeklyTimers];");
        }
        
        public override void Down()
        {
        }
    }
}
