DECLARE @startOfWeek date, @endOfWeek date;
select @startOfWeek = DATEADD(dd, -(DATEPART(dw, @week)-1), @week) 
select @endOfWeek = DATEADD(dd, 7-(DATEPART(dw, @week)), @week); 

with timerSummary as
(
    select  @employeeID EmployeeID, et.[Day], ltrim(str(sum(dbo.GetTimeSpanHours(ete.ClockIn, ete.ClockOut)), 5,2)) TotalHours,
		 ltrim(str(sum(dbo.GetTimeSpanHours(ete.ClockIn, ete.ClockOut)*60), 5,0)) TotalMinutes
    from EmployeeTimers et 
    left join EmployeeTimerEntries ete on ete.EmployeeTimerID = et.EmployeeTimerID
    where et.Day between @startOfWeek and @endOfWeek
    and ete.clockout is not null
    and et.EmployeeID = @employeeID
    group by et.EmployeeID, et.[day]
),
weekDays as
(
select DATEADD(dd, n-1, @startOfWeek) [Day]
from dbo.Nums 
where n <= 7
)
select @employeeID EmployeeID, CONVERT(nvarchar(10),wd.Day, 101) [Day], isnull(ts.TotalHours, 0) TotalHours, isnull(ts.TotalMinutes, 0) TotalMinutes
from weekDays wd
left join timerSummary ts on wd.Day = ts.Day
order by day