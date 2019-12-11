with EmployeeTimersCte as
(
	select isnull(e.EmployeeNumber, e.Name) EmployeeNumber, CONVERT(nvarchar(10),et.Day, 101) [Day], t.TimesheetID, 
		sum(isnull(dbo.GetTimeSpanHours(ete.ClockIn, ete.ClockOut), 0)) TotalHours, e.Name
	from EmployeeTimers et
	join EmployeeTimerEntries ete on et.EmployeeTimerID = ete.EmployeeTimerID
	join Employees e on et.EmployeeID = e.EmployeeID
	join Timesheets t on et.TimesheetID = t.TimesheetID
	where et.[Day] between @startDate and @endDate
	and et.departmentid = 1
	and (e.EmployeeNumber != 'Admin' or e.EmployeeNumber is null)
	group by e.EmployeeNumber, e.Name, et.Day, t.TimesheetID
),
ConcreteJobTimers as
(
	select isnull(e.EmployeeNumber, e.Name) EmployeeNumber, CONVERT(nvarchar(10),et.Day, 101) [Day], jt.TimesheetID,
			convert(decimal(10,2), sum(convert(decimal(10,4), ejt.LaborMinutes)/60.00)) TotalHours
	from EmployeeJobTimers ejt
	join JobTimers jt on ejt.JobTimerID = jt.JobTimerID
	join EmployeeTimers et on et.EmployeeTimerID = ejt.EmployeeTimerID
	join Employees e on et.EmployeeId = e.EmployeeID
	where et.[Day] between @startDate and @endDate
	and et.departmentid = 1
	and (e.EmployeeNumber != 'Admin' or e.EmployeeNumber is null)
	group by jt.TimesheetID, e.EmployeeNumber, e.Name, et.Day
),
ConcreteJobEquipmentTimers as
(
select isnull(e.EmployeeNumber, e.Name) EmployeeNumber, CONVERT(nvarchar(10),et.Day, 101) [Day], jt.TimesheetID,
			convert(decimal(10,2), sum(convert(decimal(10,4), ejet.EquipmentMinutes)/60.00)) TotalHours
	from EmployeeJobEquipmentTimers ejet
	join EmployeeJobTimers ejt on ejet.EmployeeJobTimerID = ejt.EmployeeJobTimerID
	join JobTimers jt on ejt.JobTimerID = jt.JobTimerID
	join EmployeeTimers et on et.EmployeeTimerID = ejt.EmployeeTimerID
	join Employees e on et.EmployeeId = e.EmployeeID
	where et.[Day] between @startDate and @endDate
	and et.departmentid = 1
	and (e.EmployeeNumber != 'Admin' or e.EmployeeNumber is null)
	group by jt.TimesheetID, e.EmployeeNumber, e.Name, et.Day
)

select
	etc.Name,
	etc.[Day],
	etc.TotalHours,
	isnull(cjt.TotalHours, 0) LaborHours,
	isnull(cjet.TotalHours, 0) EquipmentHours,
	isnull(cjt.TotalHours, 0) + isnull(cjet.TotalHours, 0) AllocatedHours, 
	etc.TotalHours - isnull(cjt.TotalHours, 0) - isnull(cjet.TotalHours, 0) HoursDifference,
	(etc.TotalHours - isnull(cjt.TotalHours, 0) - isnull(cjet.TotalHours, 0))*3600 SecondsDifference,	
	t.timesheetID
from EmployeeTimersCte etc
left join ConcreteJobTimers cjt on etc.TimesheetID = cjt.TimesheetID and etc.EmployeeNumber = cjt.EmployeeNumber
left join ConcreteJobEquipmentTimers cjet on etc.TimesheetID = cjet.TimesheetID and etc.EmployeeNumber = cjet.EmployeeNumber
join Timesheets t on cjt.TimesheetID = t.TimesheetID
join Employees e on t.EmployeeID = e.EmployeeID
where abs(etc.TotalHours - isnull(cjt.TotalHours, 0) - isnull(cjet.TotalHours, 0))*3600 > 60 -- if the seconds are greater than 60
and t.[day] between @startDate and @endDate;