DECLARE @StartOfWeek DATE, 
        @EndOfWeek   DATE;
declare @AllDepartments bit,
	@OwnDepartments bit;

SELECT @StartOfWeek = Dateadd(dd, -( Datepart(dw, @Week) - 1 ), @Week);
SELECT @EndOfWeek = Dateadd(dd, 7 - ( Datepart(dw, @Week) ), @Week); 
set @AllDepartments = 0;
set @OwnDepartments = 0;


select 
	@AllDepartments = case when @AllDepartments = 1 or raa.AllDepartments = 1 then 1 else 0 end,
	@OwnDepartments = case when @OwnDepartments = 1 or raa.OwnDepartments = 1 then 1 else 0 end
from RoleAuthActivities raa
	join EmployeeRoles er on er.RoleId = raa.RoleID
	join AuthActivities aa on aa.AuthActivityID = raa.AuthActivityID
where er.EmployeeID = @currentUserId
	and aa.[Key] = @viewPermissionKey;

with Permissions as
(
	select 
		raa.RoleAuthActivityID,
		raa.AuthActivityID
	from RoleAuthActivities raa
		join EmployeeRoles er on er.RoleId = raa.RoleID
		join AuthActivities aa on aa.AuthActivityID = raa.AuthActivityID
	where er.EmployeeID = @currentUserId
		and aa.[Key] = @viewPermissionKey
)

, DepartmentPermissions as
(
	select distinct
		raad.DepartmentID as DepartmentIDPermission
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

,timeCards as 
(
	select distinct etc.EmployeeTimecardID
	from EmployeeTimecards etc
	join EmployeeTimers et on etc.EmployeeTimecardID = et.EmployeeTimecardID
	join Employees e on etc.EmployeeID = e.EmployeeID
	left join Timesheets ts on et.TimesheetID = ts.TimesheetID
	where
		e.employeetypeid <> 3 
		AND e.status = 1 
		AND etc.startofweek = @StartOfWeek 
		AND ( @accountingApproved IS NULL 
			OR ( EXISTS (SELECT * 
				FROM employeetimers et 
					JOIN DailyApprovals da ON et.EmployeeID = da.EmployeeID AND et.Day = da.Day AND et.DepartmentID = da.DepartmentID
				WHERE  da.approvedbyaccounting = @accountingApproved 
					AND et.employeetimecardid = etc.employeetimecardid) ) )
		AND ( @supervisorApproved IS NULL 
			OR ( EXISTS (SELECT * 
				FROM employeetimers et 
					JOIN DailyApprovals da ON et.EmployeeID = da.EmployeeID AND et.Day = da.Day AND et.DepartmentID = da.DepartmentID
				WHERE da.approvedbysupervisor = @supervisorApproved 
					AND et.employeetimecardid = etc.employeetimecardid) ) ) 
		AND ( @billingApproved IS NULL 
			OR ( EXISTS (SELECT * 
				FROM   employeetimers et 
					JOIN DailyApprovals da ON et.EmployeeID = da.EmployeeID AND et.Day = da.Day AND et.DepartmentID = da.DepartmentID
				WHERE  da.approvedbyaccounting = @billingApproved 
					AND et.employeetimecardid = etc.employeetimecardid) ) ) 
		AND ( @departmentID IS NULL 
			OR etc.subdepartmentid = @departmentID ) 
		and (@viewPermissionKey != 'ViewBillingTimers' 
			or etc.DepartmentID not in (5, 6, 7)) --Mechanic, FrontOffice, or TMCrushing
)

select et.*
from EmployeeTimecards et
	join timeCards tc on tc.EmployeeTimecardID = et.EmployeeTimecardID
	left join DepartmentPermissions dp on dp.DepartmentIDPermission = et.DepartmentID
	left join OwnDepartmentPermissions odp on odp.OwnDepartmentIDPermission = et.DepartmentID
	left join SupervisorPermissions sp on sp.EmployeeID = et.EmployeeID
where 
	dp.DepartmentIDPermission = et.DepartmentID
	or odp.OwnDepartmentIDPermission = et.DepartmentID
	or @AllDepartments = 1
	or sp.EmployeeID = et.EmployeeID;