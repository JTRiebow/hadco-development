with JobNumberForDevelopmentOverheadRows as
(
	select
		et.EmployeeTimerID
		, j.JobNumber
		, ROW_NUMBER() OVER(PARTITION BY et.EmployeeTimerID order by ejt.CreatedOn) AS rk
	from EmployeeTimers et
		join Timesheets ts on et.TimesheetID = ts.TimesheetID
		join EmployeeJobTimers ejt on ejt.EmployeeTimerID = et.EmployeeTimerID
		join JobTimers jt on jt.JobTimerID = ejt.JobTimerID
		join Jobs j on jt.JobID = j.JobID
	where
		ts.[Day] between @startDate and @endDate
		and ts.DepartmentID in (2) --Development
		and (ts.departmentID in @departmentID)
)

, JobNumberForDevelopmentOverhead as
(
	select *
	from JobNumberForDevelopmentOverheadRows
	where rk = 1
)

, JobTimersCTE as
(
	--Concrete, Development and Residential- Invoice Timers (All Employees in CDR)
	--EmployeeNumber (P. SAMA), Name, Day, JobNumber, PhaseNumber, CategoryNumber, Labor Hours, EquipmentNumber, Equipcode (r, re, etc.), EquipmentHours, Total Hours, Department, Invoice Number
	--CDR LABOR HOURS
	select 
		isNull(e.EmployeeNumber, e.[Name]) Employee
		, e.[Name]
		, cast(ts.[Day] as nvarchar(10)) Day
		, c.JobNumber
		, c.PhaseNumber
		, c.CategoryNumber
		, ejt.LaborMinutes / 60.0 LaborHours
		, '' EquipmentNumber
		, '' EquipmentCode
		, 0 EquipmentHours
		, ejt.LaborMinutes / 60.0 TotalHours
		, 'FIELD WAGE' Department
		, jt.InvoiceNumber
		, d.[Name] DepartmentName
		, '' [Type]
		, replace(jt.Diary, '’', '''') JobDiaryNote
	from EmployeeTimers et
		join Timesheets ts on et.TimesheetID = ts.TimesheetID
		join EmployeeJobTimers ejt on ejt.EmployeeTimerID = et.EmployeeTimerID
		join JobTimers jt on jt.JobTimerID = ejt.JobTimerID
		left join Categories c on c.CategoryID = jt.CategoryID
		join Employees e on e.EmployeeID = et.EmployeeID
		join Departments d on d.DepartmentID = ts.DepartmentID
	where
		ts.[Day] between @startDate and @endDate
		and ts.DepartmentID in (1, 2, 3, 9, 10) --Concrete, Development, Residential, Concrete2H, ConcreteHB
		and (ts.departmentID in @departmentID)

	union all
	
	--Concrete, Development and Residential- Invoice Timers (All Employees in CDR)
	--EmployeeNumber (P. SAMA), Name, Day, JobNumber, PhaseNumber, CategoryNumber, Labor Hours, EquipmentNumber, Equipcode (r, re, etc.), EquipmentHours, Total Hours, Department, Invoice Number
	--CDR EQUIPMENT HOURS
	select 
		isNull(e.EmployeeNumber, e.[Name]) Employee
		, e.[Name]
		, cast(ts.[Day] as nvarchar(10)) Day
		, c.JobNumber
		, c.PhaseNumber
		, c.CategoryNumber
		, 0 LaborHours
		, eq.EquipmentNumber
		, 'R' EquipmentCode
		, ejet.EquipmentMinutes / 60.0 EquipmentHours
		, ejet.EquipmentMinutes / 60.0 TotalHours
		, 'FIELD WAGE' Department
		, jt.InvoiceNumber
		, d.[Name] DepartmentName
		, '' [Type]
		, replace(jt.Diary, '’', '''') JobDiaryNote
	from EmployeeTimers et
		join Timesheets ts on et.TimesheetID = ts.TimesheetID
		join EmployeeJobTimers ejt on ejt.EmployeeTimerID = et.EmployeeTimerID
		join EmployeeJobEquipmentTimers ejet on ejet.EmployeeJobTimerID = ejt.EmployeeJobTimerID
		left join Equipment eq on eq.EquipmentID = ejet.EquipmentID
		join JobTimers jt on jt.JobTimerID = ejt.JobTimerID
		left join Categories c on c.CategoryID = jt.CategoryID
		join Employees e on e.EmployeeID = et.EmployeeID
		join Departments d on d.DepartmentID = ts.DepartmentID
	where
		ts.[Day] between @startDate and @endDate
		and ts.DepartmentID in (1, 2, 3, 9, 10) --Concrete, Development, Residential, Concrete2H, ConcreteHB
		and (ts.departmentID in @departmentID)

	union all

	--Concrete, Development and Residential- Invoice Timers (All Employees in CDR)
	--EmployeeNumber (P. SAMA), Name, Day, JobNumber, PhaseNumber, CategoryNumber, Labor Hours, EquipmentNumber, Equipcode (r, re, etc.), EquipmentHours, Total Hours, Department, Invoice Number
	--CDR OVERHEAD SHOP HOURS
	select 
		isNull(e.EmployeeNumber, e.[Name]) Employee
		, e.[Name]
		, cast(ts.[Day] as nvarchar(10)) Day
		, case when et.DepartmentID in (2) then jndo.JobNumber
			else ohc.JobNumber
		end JobNumber
		, ohc.PhaseNumber
		, ohc.CategoryNumber
		, 0 LaborHours
		, '' EquipmentNumber
		, '' EquipmentCode
		, 0 EquipmentHours
		, et.ShopMinutes / 60.0 TotalHours
		, 'FIELD WAGE' Department
		, '' InvoiceNumber
		, d.[Name] DepartmentName
		, ohc.[Type]
		, null JobDiaryNote
	from EmployeeTimers et
		join Timesheets ts on et.TimesheetID = ts.TimesheetID
		join OverheadCodes ohc on ohc.DepartmentID = et.DepartmentID
		left join JobNumberForDevelopmentOverhead jndo on jndo.EmployeeTimerID = et.EmployeeTimerID
		join Employees e on e.EmployeeID = et.EmployeeID
		join Departments d on d.DepartmentID = et.DepartmentID
	where
		et.[Day] between @startDate and @endDate
		and ts.DepartmentID in (1, 2, 3, 9, 10) --Concrete, Development, Residential, Concrete2H, ConcreteHB
		and (ts.departmentID in @departmentID)
		and ohc.[Type] = 'Shop'
		and et.ShopMinutes != 0

	union all

	--Concrete, Development and Residential- Invoice Timers (All Employees in CDR)
	--EmployeeNumber (P. SAMA), Name, Day, JobNumber, PhaseNumber, CategoryNumber, Labor Hours, EquipmentNumber, Equipcode (r, re, etc.), EquipmentHours, Total Hours, Department, Invoice Number
	--CDR OVERHEAD GREASE HOURS
	select 
		isNull(e.EmployeeNumber, e.[Name]) Employee
		, e.[Name]
		, cast(ts.[Day] as nvarchar(10)) Day
		, case when et.DepartmentID in (2) then jndo.JobNumber
			else ohc.JobNumber
		end JobNumber
		, ohc.PhaseNumber
		, ohc.CategoryNumber
		, 0 LaborHours
		, '' EquipmentNumber
		, '' EquipmentCode
		, 0 EquipmentHours
		, et.GreaseMinutes / 60.0 TotalHours
		, 'FIELD WAGE' Department
		, '' InvoiceNumber
		, d.[Name] DepartmentName
		, ohc.[Type]
		, null JobDiaryNote
	from EmployeeTimers et
		join Timesheets ts on et.TimesheetID = ts.TimesheetID
		join OverheadCodes ohc on ohc.DepartmentID = et.DepartmentID
		left join JobNumberForDevelopmentOverhead jndo on jndo.EmployeeTimerID = et.EmployeeTimerID
		join Employees e on e.EmployeeID = et.EmployeeID
		join Departments d on d.DepartmentID = et.DepartmentID
	where
		et.[Day] between @startDate and @endDate
		and ts.DepartmentID in (1, 2, 3, 9, 10) --Concrete, Development, Residential, Concrete2H, ConcreteHB
		and (ts.departmentID in @departmentID)
		and ohc.[Type] = 'Grease'
		and et.GreaseMinutes != 0

	union all

	--Concrete, Development and Residential- Invoice Timers (All Employees in CDR)
	--EmployeeNumber (P. SAMA), Name, Day, JobNumber, PhaseNumber, CategoryNumber, Labor Hours, EquipmentNumber, Equipcode (r, re, etc.), EquipmentHours, Total Hours, Department, Invoice Number
	--CDR OVERHEAD DAILY HOURS
	select 
		isNull(e.EmployeeNumber, e.[Name]) Employee
		, e.[Name]
		, cast(ts.[Day] as nvarchar(10)) Day
		, case when et.DepartmentID in (2) then jndo.JobNumber
			else ohc.JobNumber
		end JobNumber
		, ohc.PhaseNumber
		, ohc.CategoryNumber
		, 0 LaborHours
		, '' EquipmentNumber
		, '' EquipmentCode
		, 0 EquipmentHours
		, et.DailyMinutes / 60.0 TotalHours
		, 'FIELD WAGE' Department
		, '' InvoiceNumber
		, d.[Name] DepartmentName
		, ohc.[Type]
		, null JobDiaryNote
	from EmployeeTimers et
		join Timesheets ts on et.TimesheetID = ts.TimesheetID
		join OverheadCodes ohc on ohc.DepartmentID = et.DepartmentID
		left join JobNumberForDevelopmentOverhead jndo on jndo.EmployeeTimerID = et.EmployeeTimerID
		join Employees e on e.EmployeeID = et.EmployeeID
		join Departments d on d.DepartmentID = et.DepartmentID
	where
		et.[Day] between @startDate and @endDate
		and ts.DepartmentID in (1, 2, 3, 9, 10) --Concrete, Development, Residential, Concrete2H, ConcreteHB
		and (ts.departmentID in @departmentID)
		and ohc.[Type] = 'Daily'
		and et.DailyMinutes != 0

	union all

	--Concrete, Development and Residential- Invoice Timers (All Employees in CDR)
	--EmployeeNumber (P. SAMA), Name, Day, JobNumber, PhaseNumber, CategoryNumber, Labor Hours, EquipmentNumber, Equipcode (r, re, etc.), EquipmentHours, Total Hours, Department, Invoice Number
	--CDR OVERHEAD TRAVEL HOURS
	select 
		isNull(e.EmployeeNumber, e.[Name]) Employee
		, e.[Name]
		, cast(ts.[Day] as nvarchar(10)) Day
		, case when et.DepartmentID in (2) then jndo.JobNumber
			else ohc.JobNumber
		end JobNumber
		, ohc.PhaseNumber
		, ohc.CategoryNumber
		, 0 LaborHours
		, '' EquipmentNumber
		, '' EquipmentCode
		, 0 EquipmentHours
		, et.TravelMinutes / 60.0 TotalHours
		, 'FIELD WAGE' Department
		, '' InvoiceNumber
		, d.[Name] DepartmentName
		, ohc.[Type]
		, null JobDiaryNote
	from EmployeeTimers et
		join Timesheets ts on et.TimesheetID = ts.TimesheetID
		join OverheadCodes ohc on ohc.DepartmentID = et.DepartmentID
		left join JobNumberForDevelopmentOverhead jndo on jndo.EmployeeTimerID = et.EmployeeTimerID
		join Employees e on e.EmployeeID = et.EmployeeID
		join Departments d on d.DepartmentID = et.DepartmentID
	where
		et.[Day] between @startDate and @endDate
		and ts.DepartmentID in (1, 2, 3, 9, 10) --Concrete, Development, Residential, Concrete2H, ConcreteHB
		and (ts.departmentID in @departmentID)
		and ohc.[Type] = 'Travel'
		and et.TravelMinutes != 0
	
	union all

	--Mechanics- Job Timers (All Employees in Mechanics)
	--EmployeeNumber(P. SAMA), Day, TotalHours, JobNumber, PhaseNumber, CategoryNumber, Department
	--EQUIPMENT TIME COMES FROM DIFFERENT QUERY, Do not query the equipment time here.
	--MECHANIC DOWNTIME HOURS
	select 
		isnull(e.EmployeeNumber, e.[Name]) Employee
		, e.[Name]
		, cast(ts.[Day] as nvarchar(10)) Day
		, c.JobNumber
		, c.PhaseNumber
		, c.CategoryNumber
		, 0 LaborHours
		, '' EquipmentNumber
		, '' EquipmentCode
		, 0 EquipmentHours
		, isNull(sum(DateDiff(minute, jt.StartTime, jt.StopTime)), 0) / 60.0 TotalHours
		, 'FIELD WAGE' Department
		, jt.InvoiceNumber
		, d.[Name] DepartmentName
		, '' [Type]
		, replace(jt.Diary, '’', '''') JobDiaryNote
	from Timesheets ts
		left join JobTimers jt on jt.TimesheetID = ts.TimesheetID
		left join EmployeeJobTimers ejt on ejt.JobTimerID = jt.JobTimerID
		left join Categories c on c.CategoryID = jt.CategoryID
		join Employees e on e.EmployeeID = ts.EmployeeID
		join Departments d on d.DepartmentID = ts.DepartmentID
	where 
		ts.[Day] between @startDate and @endDate
		and ts.DepartmentID in (5) --Mechanics
		and (ts.departmentID in @departmentID)
	group by 
		e.EmployeeNumber
		, e.[Name]
		, ts.[Day]
		, c.JobNumber
		, c.PhaseNumber
		, c.CategoryNumber
		, ejt.LaborMinutes
		, ejt.EquipmentMinutes
		, jt.StartTime, jt.StopTime
		, jt.InvoiceNumber
		, d.[Name]
		, jt.Diary
)

select *
from JobTimersCTE
where
	TotalHours != 0
order by [Day], Employee, InvoiceNumber, JobNumber, PhaseNumber, CategoryNumber;