SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create function [dbo].[GetCustomerType]
(
	@JobClass nvarchar(128), @JobNumber nvarchar(128)
)
returns int
as
BEGIN
declare @CustomerTypeID int;
	select @CustomerTypeID = ct.CustomerTypeID 
from CustomerTypes ct
where case when @JobClass like 'P%'then 'Development' 
		when @JobClass like 'L%' or @JobClass like 'F%' or @JobClass like 'J%' then 'Residential'
		when @JobClass like 'S%' and @JobNumber = 'METRO' then 'Metro'
		when @JobClass like 'S%' and @JobNumber = 'TKMISC' then 'Outside' 
	end = ct.Name
	return @CustomerTypeID
END