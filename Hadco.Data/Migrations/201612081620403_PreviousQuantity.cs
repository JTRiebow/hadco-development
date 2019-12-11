namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PreviousQuantity : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                    SET ANSI_NULLS ON
                    GO
                    SET QUOTED_IDENTIFIER ON
                    GO
                    -- =============================================
                    -- Author:		Brandon Warner
                    -- Create date: 12/09/2016
                    -- =============================================
                    CREATE FUNCTION GetPreviousQuantity 
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
                    
                    END
                    GO");
            Sql("ALTER TABLE JobTimers ADD PreviousQuantity AS dbo.GetPreviousQuantity(CategoryID, StartTime)");
        }
        
        public override void Down()
        {
            DropColumn("dbo.JobTimers", "PreviousQuantity");
            Sql("DROP FUNCTION dbo.GetPreviousQuantity");
        }
    }
}
