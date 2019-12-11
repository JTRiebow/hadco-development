with EmployeeTimersCte as
(
	select sum(dbo.GetTimespanHours(ete.ClockIn, ete.ClockOut)) TotalHours, t.TimesheetID
	from Timesheets t
	join EmployeeTimers et on t.TimesheetID = et.TimesheetID
	join EmployeeTimerEntries ete on et.EmployeeTimerID = ete.EmployeeTimerID
	where ete.ClockOut is not null
	and ete.ClockIn > '2000-01-01' -- some dates are in the system that are way old and cause an overflow on the datediff.
	and t.[day] between @startDate and @endDate
	group by t.TimesheetID
),
LoadAndDowntimeTimers as
(
	select sum(TotalHours) TotalHours, TimesheetID 
	from (select sum(dbo.GetTimespanHours(lte.StartTime, lte.EndTime)) TotalHours, lt.TimesheetID
			from LoadTimers lt
			left join LoadTimerEntries lte on lt.LoadTimerID = lte.LoadTimerID
			join Timesheets t on lt.TimesheetID = t.TimesheetID
			where lte.StartTime is not null
			and lte.EndTime is not null
			and lte.StartTime > '2000-01-01'
			and t.[day] between @startDate and @endDate
			group by lt.TimesheetID
			union all
			select sum(dbo.GetTimespanHours(dt.StartTime, dt.StopTime)), dt.TimesheetID
			from DowntimeTimers dt 
			join Timesheets t on dt.TimesheetID = t.TimesheetID
			where dt.StartTime is not null
			and dt.StopTime is not null
			and dt.StartTime > '2000-01-01'
			and t.[day] between @startDate and @endDate
			group by dt.TimesheetID) t
	group by t.TimesheetID
)

select	e.Name,
		CONVERT(nvarchar(10),t.Day, 101) [Day],
		etc.TotalHours,
		lat.TotalHours AllocatedHours,
		etc.TotalHours - lat.TotalHours HoursDifference,
		((etc.TotalHours - lat.TotalHours)*3600) SecondsDifference, 
		t.TimesheetID
from EmployeeTimersCte etc
join LoadAndDowntimeTimers lat on etc.TimesheetID = lat.TimesheetID
join Timesheets t on lat.TimesheetID = t.TimesheetID
join Employees e on t.EmployeeID = e.EmployeeID
where abs((etc.TotalHours - lat.TotalHours)*3600) > 60