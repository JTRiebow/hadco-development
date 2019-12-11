namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOtherNewQuantity : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.JobTimers", "OtherNewQuantity", c => c.Decimal(precision: 18, scale: 2));

            Sql(@"
            CREATE FUNCTION [dbo].[GetOtherNewQuantity] 
                    (
                    	-- Add the parameters for the function here
                    	@JobTimerID int
                    )
                    RETURNS decimal
                    AS
                    BEGIN
                    	-- Declare the return variable here
                    	DECLARE @Result decimal
                    
                    	-- Add the T-SQL statements to compute the return value here
                    	select @Result = sum(jt2.NewQuantity) 
						from JobTimers jt
						join Timesheets t on jt.TimesheetID = t.TimesheetID
						join JobTimers jt2 on jt2.CategoryID = jt.CategoryID
						join Timesheets t2 on jt2.TimesheetID = t2.TimesheetID and t.Day = t2.Day and t.TimesheetID != t2.TimesheetID
						where @JobTimerID = jt.JobTimerID
                    
                    	-- Return the result of the function
                    	RETURN @Result
                    
                    END
");

            Sql(@"ALTER TABLE JobTimers ADD OtherNewQuantity AS dbo.[GetOtherNewQuantity](JobTimerID)");

            Sql(@"ALTER TABLE JobTimers DROP COLUMN [PreviousQuantity]");

            Sql(@"-- =============================================
                    -- Author:		Brandon Warner
                    -- Create date: 12/09/2016
                    -- =============================================
                    ALTER FUNCTION [dbo].[GetPreviousQuantity] 
                    (
                    	-- Add the parameters for the function here
                    	@JobTimerID int
                    )
                    RETURNS decimal
                    AS
                    BEGIN
                    	-- Declare the return variable here
                    	DECLARE @Result decimal
                    
                    	-- Add the T-SQL statements to compute the return value here
                    	select @Result = sum(jt2.NewQuantity) 
						from JobTimers jt
						join Timesheets t on jt.TimesheetID = t.TimesheetID
						join JobTimers jt2 on jt2.CategoryID = jt.CategoryID
						join Timesheets t2 on jt2.TimesheetID = t2.TimesheetID
						where @JobTimerID = jt.JobTimerID and t2.Day < t.Day
                    
                    	-- Return the result of the function
                    	RETURN @Result
                    
                    END");
            Sql(@"ALTER TABLE JobTimers ADD PreviousQuantity AS dbo.GetPreviousQuantity(JobTimerID)");
        }
        
        public override void Down()
        {
            DropColumn("dbo.JobTimers", "OtherNewQuantity");
            Sql("DROP FUNCTION dbo.GetOtherNewQuantity");

            Sql(@"ALTER TABLE JobTimers DROP COLUMN [PreviousQuantity]");
            Sql(@"ALTER FUNCTION GetPreviousQuantity 
                    (
                    	-- Add the parameters for the function here
                    	@CategoryID int, @StartTime DateTimeOffset
                    )
                    RETURNS decimal
                    AS
                    BEGIN
                    	-- Declare the return variable here
                    	DECLARE @Result decimal
                    
                    	-- Add the T-SQL statements to compute the return value here
                    	select @Result = sum(jt.NewQuantity) from JobTimers jt where @CategoryID = jt.CategoryID and jt.StartTime < @StartTime
                    
                    	-- Return the result of the function
                    	RETURN @Result
                    
                    END");
            Sql(@"ALTER TABLE JobTimers ADD PreviousQuantity AS dbo.GetPreviousQuantity(CategoryID, StartTime)");
        }
    }
}
