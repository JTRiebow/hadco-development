select  isnull(e.EmployeeNumber, e.Name) Employee, 
CONVERT(nvarchar(10),et.[Day], 101) [Day],
ltrim(str(sum(dbo.GetTimespanHours(ete.ClockIn, ete.ClockOut)), 5,2)) TotalHours,
d.Name Department,
p.Name Pit
from EmployeeTimecards etc 
join Employees e on etc.EmployeeID = e.EmployeeID
left join EmployeeTimers et on etc.EmployeeTimecardID = et.EmployeeTimecardID
left join EmployeeTimerEntries ete on ete.EmployeeTimerID = et.EmployeeTimerID
left join Pits p on ete.PitID = p.PitID
join Departments d on d.DepartmentID = etc.SubDepartmentID
left join DailyApprovals da on da.Day = et.Day and et.EmployeeID = da.EmployeeID and et.DepartmentID = da.DepartmentID
where 1=1 
and et.[Day] between @startDate and @endDate
and (etc.SubDepartmentID in @departmentID)
and ete.clockout is not null and ete.clockout > '2000-01-01'
and ete.clockin > '2000-01-01'
	and da.ApprovedByAccounting = 1 and da.ApprovedBySupervisor = 1
and (e.EmployeeNumber != 'Admin' or e.EmployeeNumber is null)
group by e.EmployeeNumber, e.Name, et.[day], d.Name, p.Name