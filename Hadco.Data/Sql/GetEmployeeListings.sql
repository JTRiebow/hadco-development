declare @AllDepartments bit,
	@OwnDepartments bit;
set @AllDepartments = 0;
set @OwnDepartments = 0;

select 
	@AllDepartments = case when @AllDepartments = 1 or raa.AllDepartments = 1 then 1 else 0 end,
	@OwnDepartments = case when @OwnDepartments = 1 or raa.OwnDepartments = 1 then 1 else 0 end
from RoleAuthActivities raa
	join EmployeeRoles er on er.RoleId = raa.RoleID
	join AuthActivities aa on aa.AuthActivityID = raa.AuthActivityID
where er.EmployeeID = @currentUserId
	and aa.[Key] in ('ViewEmployeeList');

with Permissions as
(
	select 
		raa.RoleAuthActivityID,
		raa.AuthActivityID
	from RoleAuthActivities raa
		join EmployeeRoles er on er.RoleId = raa.RoleID
		join AuthActivities aa on aa.AuthActivityID = raa.AuthActivityID
	where er.EmployeeID = @currentUserId
		and aa.[Key] in ('ViewEmployeeList')
)

, DepartmentPermissions as
(
	select raad.DepartmentID as DepartmentIDPermission
	from RoleAuthActivityDepartments raad
		join Permissions p on p.RoleAuthActivityID = raad.RoleAuthActivityID
)

, OwnDepartmentPermissions as
(
	select distinct
		d.DepartmentID as OwnDepartmentIDPermission
	from Departments d 
		join EmployeeDepartments ed on ed.DepartmentID = d.DepartmentID and @currentUserId = ed.EmployeeID
	where @OwnDepartments = 1
)

, SupervisorPermissions as
(
	select distinct
		e.EmployeeID
	from Employees e
		join EmployeeSupervisors es on es.EmployeeID = e.EmployeeID
	where es.SupervisorID = @currentUserId
)


select distinct
	e.EmployeeId as ID,
	e.[Name],
	e.Username,
	cis.IsClockedIn as ClockedInStatusBit,
	e.[Location]
from Employees e
	left join EmployeeDepartments ed on ed.EmployeeID = e.EmployeeID
	left join DepartmentPermissions dp on dp.DepartmentIDPermission = ed.DepartmentID
	left join OwnDepartmentPermissions owd on owd.OwnDepartmentIDPermission = ed.DepartmentID
	left join SupervisorPermissions sp on sp.EmployeeID = e.EmployeeID
	left join ClockedInStatuses cis on cis.EmployeeID = e.EmployeeID
where
	e.[Status] = 1 and
	(dp.DepartmentIDPermission = ed.DepartmentID
	or owd.OwnDepartmentIDPermission = ed.DepartmentID
	or @AllDepartments = 1
	or sp.EmployeeID = e.EmployeeID)
order by
	e.[Name];