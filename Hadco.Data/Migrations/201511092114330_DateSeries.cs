namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateSeries : DbMigration
    {
        public override void Up()
        {
            Sql(@"IF OBJECT_ID('dbo.GetDateSeries') IS NOT NULL
                      DROP FUNCTION dbo.GetDateSeries;
                    GO
                    CREATE FUNCTION dbo.GetDateSeries(@startdate AS DATE, @enddate AS DATE) RETURNS TABLE
                    AS
                    RETURN
	                    SELECT DATEADD(day, n - 1, @startdate) AS day
	                    FROM dbo.Nums
	                    WHERE n <= DATEDIFF(day, @startdate, @enddate) + 1;
                    GO");
        }
        
        public override void Down()
        {
            Sql(@"IF OBJECT_ID('dbo.GetDateSeries') IS NOT NULL
                      DROP FUNCTION dbo.GetDateSeries; ");
        }
    }
}
