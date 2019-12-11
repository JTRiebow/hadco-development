namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AlterGetTimespanHoursAndTruckerDaily : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.TruckerDailies", "DowntimeTimerID", c => c.Int());

            //Sql(@"IF OBJECT_ID('TruckerDailies', 'view') IS NOT NULL
            //                    DROP VIEW TruckerDailies;");
            //Sql(Data.Resources.CreateTruckerDailyView);
            Sql(@"IF OBJECT_ID('GetTimeSpanHours', 'function') IS NOT NULL
                /****** Object:  UserDefinedFunction [dbo].[GetTimeSpanHours]    Script Date: 6/30/2016 11:07:34 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                ALTER function [dbo].[GetTimeSpanHours]
                (
	                @StartTime DateTimeOffset,
	                @EndTime DateTimeOffset
                )
                returns decimal(15,5)
                as
                BEGIN
	                return case when @StartTime < '20000101' or @EndTime < '20000101' then 0 
						else  abs(convert(decimal(15,5), convert(decimal(15,5), datediff(ss, @StartTime, @EndTime))/convert(decimal(15,5),3600.00))) end;
                END");
        }

        public override void Down()
        {
            //DropColumn("dbo.TruckerDailies", "DowntimeTimerID");

            Sql(@"ALTER function [dbo].[GetTimeSpanHours]
                (
	                @StartTime DateTimeOffset,
	                @EndTime DateTimeOffset
                )
                returns decimal(15,5)
                as
                BEGIN
	                return abs(convert(decimal(15,5), convert(decimal(15,5), datediff(ss, @StartTime, @EndTime))/convert(decimal(15,5),3600.00)));
                END");
        }
    }
}