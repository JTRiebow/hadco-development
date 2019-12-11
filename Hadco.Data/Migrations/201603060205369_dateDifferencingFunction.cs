namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateDifferencingFunction : DbMigration
    {
        public override void Up()
        {
            Sql(@"SET ANSI_NULLS ON
                GO

                SET QUOTED_IDENTIFIER ON
                GO

                create function [dbo].[GetTimeSpanHours]
                (
	                @StartTime DateTimeOffset,
	                @EndTime DateTimeOffset
                )
                returns decimal(15,5)
                as
                BEGIN
	                return abs(convert(decimal(15,5), convert(decimal(15,5), datediff(ss, @StartTime, @EndTime))/convert(decimal(15,5),3600.00)));
                END
                GO");
        }
        
        public override void Down()
        {
            Sql("drop function [dbo].[GetTimeSpanHours]");
        }
    }
}
