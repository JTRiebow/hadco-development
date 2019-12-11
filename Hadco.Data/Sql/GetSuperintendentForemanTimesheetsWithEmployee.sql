declare @startOfWeek date, @endOfWeek date;
select @startOfWeek = DATEADD(dd, -(DATEPART(dw, @week)-1), @week) 
select @endOfWeek = DATEADD(dd, 7-(DATEPART(dw, @week)), @week);

select e.EmployeeID, e.Name, min(ed.DepartmentID) DepartmentID--default department to concrete, otherwise residential then development
from Employees e
join EmployeeSupervisors es on es.EmployeeID = e.EmployeeID
join EmployeeRoles er on er.EmployeeID = e.EmployeeID
join EmployeeDepartments ed on e.EmployeeID = ed.EmployeeID
join Timesheets t on e.EmployeeID = t.EmployeeID
join EmployeeTimers et on et.TimesheetID = t.TimesheetID
where et.EmployeeID = @employeeID
and er.RoleID = 7
and e.[Status] = 1
and t.Day between @startOfWeek and @endOfWeek
group by e.EmployeeID, e.Name

select e.EmployeeID, t.TimesheetID, t.DepartmentID, t.[Day]
from Employees e
join EmployeeSupervisors es on es.EmployeeID = e.EmployeeID
join EmployeeRoles er on er.EmployeeID = e.EmployeeID
join Timesheets t on e.EmployeeID = t.EmployeeID
join EmployeeTimers et on t.TimesheetID = et.TimesheetID 
where et.EmployeeID = @employeeID
and er.RoleID = 7
and e.[Status] = 1
and t.[Day] between @startOfWeek and @endOfWeek
group by e.EmployeeID, t.TimesheetID, t.DepartmentID, t.[Day]

select t.TimesheetID, j.JobNumber
from Timesheets t
join EmployeeTimers et on t.TimesheetID = et.TimesheetID
join EmployeeJobTimers ejt on et.EmployeeTimerID = ejt.EmployeeTimerID
join JobTimers jt on ejt.JobTimerID = jt.JobTimerID
join Jobs j on jt.JobID = j.JobID
where et.EmployeeID = @employeeID
and t.[Day] between @startOfWeek and @endOfWeek