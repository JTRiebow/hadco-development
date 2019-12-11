with SpecificDepartmentsList as
(
	select distinct
		e.EmployeeID,
		Substring(
			(
				select distinct
					 aa.Name + ', '
				from Employees ei
					join EmployeeRoles er on er.EmployeeID = ei.EmployeeID
					join RoleAuthActivities raa on raa.RoleID = er.RoleId
					join RoleAuthActivityDepartments raad on raad.RoleAuthActivityID = raa.RoleAuthActivityID
					join AuthActivities aa on aa.AuthActivityID = raa.AuthActivityID
				where e.EmployeeID = ei.EmployeeID 
				for xml path('')
			), 1, 1000) SpecificDepartmentsPermissionsList,
		Substring(
			(
				select distinct
					 d.Name + ', '
				from Employees ei
					join EmployeeRoles er on er.EmployeeID = ei.EmployeeID
					join RoleAuthActivities raa on raa.RoleID = er.RoleId
					join RoleAuthActivityDepartments raad on raad.RoleAuthActivityID = raa.RoleAuthActivityID
					join Departments d on d.DepartmentID = raad.DepartmentID
				where e.EmployeeID = ei.EmployeeID 
				for xml path('')
			), 1, 1000) SpecificDepartmentsList
	from Employees e
)

, OwnDepartmentsPermissionsList as
(
	select distinct
		e.EmployeeID,
		Substring(
			(
				select distinct
					aa.Name + ', '
				from Employees ei
					join EmployeeRoles er on er.EmployeeID = ei.EmployeeID
					join RoleAuthActivities raa on raa.RoleID = er.RoleId
					join AuthActivities aa on aa.AuthActivityID = raa.AuthActivityID
				where e.EmployeeID = ei.EmployeeID 
					and raa.OwnDepartments = 1
				for xml path('')
			), 1, 1000) OwnDepartmentsPermissionsList,
		Substring(
			(
				select distinct
					d.Name + ', '
				from Employees ei
					join EmployeeRoles er on er.EmployeeID = ei.EmployeeID
					join EmployeeDepartments ed on ed.EmployeeID = ei.EmployeeID
					join Departments d on d.DepartmentID = ed.DepartmentID
				where e.EmployeeID = ei.EmployeeID 
				for xml path('')
			), 1, 1000) OwnDepartmentsList
	from Employees e
)

, AllDepartmentPermissionsList as 
(
	select 
		e.EmployeeID,
		Substring(
			(
				select distinct
					 aa.Name + ', '
				from Employees ei
					join EmployeeRoles er on er.EmployeeID = ei.EmployeeID
					join RoleAuthActivities raa on raa.RoleID = er.RoleId
					join AuthActivities aa on aa.AuthActivityID = raa.AuthActivityID
				where e.EmployeeID = ei.EmployeeID 
					and raa.AllDepartments = 1
				for xml path('')
			), 1, 1000) AllDepartmentsPermissionsList
	from Employees e
)

, SupervisorsList as 
(
	select
		e.EmployeeID,
		Substring(
			(
				select distinct
					s.Name + ', '
				from Employees ei
					join EmployeeSupervisors es on es.EmployeeID = ei.EmployeeID
					join Employees s on s.EmployeeID = es.SupervisorID
				where e.EmployeeID = ei.EmployeeID 
				for xml path('')
			), 1, 1000) SupervisorsList
	from Employees e
)

, RolesList as 
(
	select
		e.EmployeeID,
		Substring(
			(
				select distinct
					 r.Name + ', '
				from Employees ei
					join EmployeeRoles er on er.EmployeeID = ei.EmployeeID
					join Roles r on r.RoleID = er.RoleId
				where e.EmployeeID = ei.EmployeeID 
				for xml path('')
			), 1, 1000) RolesList
	from Employees e
)


select distinct
  e.Name
  , e.Username
  , case when odpl.OwnDepartmentsList is null then '' else odpl.OwnDepartmentsList end as Departments
  , case when sl.SupervisorsList is null then '' else sl.SupervisorsList end as Supervisors
  , case when rl.RolesList is null then '' else rl.RolesList end as Roles
  , case when adpl.AllDepartmentsPermissionsList is null then '' else adpl.AllDepartmentsPermissionsList end AllDepartmentsPermissions
  , case when odpl.OwnDepartmentsPermissionsList is null then '' else odpl.OwnDepartmentsPermissionsList end OwnDepartmentsPermissions
  , case when odpl.OwnDepartmentsList is null then '' else odpl.OwnDepartmentsList end OwnDepartments
  , case when sdl.SpecificDepartmentsPermissionsList is null then '' else sdl.SpecificDepartmentsPermissionsList end SpecificDepartmentsPermissions
  , case when sdl.SpecificDepartmentsList is null then '' else sdl.SpecificDepartmentsList end SpecificDepartments
from Employees e
	join SupervisorsList sl on sl.EmployeeID = e.EmployeeID
	join RolesList rl on rl.EmployeeID = e.EmployeeID
	join AllDepartmentPermissionsList adpl on adpl.EmployeeID = e.EmployeeID
	join OwnDepartmentsPermissionsList odpl on odpl.EmployeeID = e.EmployeeID
	join SpecificDepartmentsList sdl on sdl.EmployeeID = e.EmployeeID
	join EmployeeDepartments ed on ed.EmployeeID = e.EmployeeID
where e.Status = 1
	and ed.DepartmentID in @departmentID
