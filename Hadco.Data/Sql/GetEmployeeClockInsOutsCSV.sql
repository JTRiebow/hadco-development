select
	e.[Name] Employee
	, ete.ClockIn
	, ete.ClockOut
	, DATEDIFF(minute, ete.ClockIn, ete.ClockOut) / 60 [Hours]
	, DateDiff(minute, ete.ClockIn, ete.ClockOut) % 60 [Minutes]
from EmployeeTimerEntries ete
	join EmployeeTimers et on et.EmployeeTimerID = ete.EmployeeTimerID
	join  Employees e on e.EmployeeID = et.EmployeeID
where 
	et.Day between @startDate and @endDate
	and et.DepartmentID in @departmentId
	and e.[Status] = 1
group by e.[Name], ete.ClockIn, ete.ClockOut, et.[Day]
order by et.[Day], [Name];