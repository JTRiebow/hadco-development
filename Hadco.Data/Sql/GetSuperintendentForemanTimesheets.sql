DECLARE @startOfWeek date, @endOfWeek date;

select @startOfWeek = DATEADD(dd, -(DATEPART(dw, @week)-1), @week) 
select @endOfWeek = DATEADD(dd, 7-(DATEPART(dw, @week)), @week);

select e.EmployeeID, e.Name, min(ed.DepartmentID) DepartmentID--default department to concrete, otherwise residential then development
from Employees e
join EmployeeSupervisors es on es.EmployeeID = e.EmployeeID
join EmployeeRoles er on er.EmployeeID = e.EmployeeID
left join EmployeeDepartments ed on e.EmployeeID = ed.EmployeeID
where es.SupervisorID = @superintendentID
and RoleID = 7
and [Status] = 1
group by e.EmployeeID, e.Name

select f.TimesheetID, f.DepartmentID, convert(VARCHAR(10), f.Day, 101) [Day], f.EmployeeID
from 
(
select e.EmployeeID, t.TimesheetID, t.DepartmentID, t.[Day]
from Employees e
join EmployeeSupervisors es on es.EmployeeID = e.EmployeeID
join EmployeeRoles er on er.EmployeeID = e.EmployeeID
left join Timesheets t on e.EmployeeID = t.EmployeeID
where es.SupervisorID = @superintendentID
and RoleID = 7
and [Status] = 1
) f
where f.[Day] between @startOfWeek and @endOfWeek

select t.TimesheetID, j.JobNumber
from Timesheets t
join EmployeeSupervisors es on es.EmployeeID = t.EmployeeID
join EmployeeTimers et on t.TimesheetID = et.TimesheetID
join EmployeeJobTimers ejt on et.EmployeeTimerID = ejt.EmployeeTimerID
join JobTimers jt on ejt.JobTimerID = jt.JobTimerID
join Jobs j on jt.JobID = j.JobID
where es.SupervisorID = @superintendentID
and t.[Day] between @startOfWeek and @endOfWeek
group by  t.TimesheetID, j.JobNumber