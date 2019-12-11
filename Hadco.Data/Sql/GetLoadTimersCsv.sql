with loads as
(
	select	e.EmployeeNumber, 
			case when j.jobnumber in ('METRO','ALTAVW') then 
				case when p.phasenumber ='GVPG' then 'METROPG'
					 when p.phasenumber = 'GVWV' then 'METROWV'
					 when p.phasenumber = 'GVTC' then 'METROTC' 
					 when p.phasenumber = 'GVWJ' then 'GVWJ'
					 when p.phasenumber = 'HKPG' then 'HKPG'
					 when p.phasenumber = 'PPTC' then 'PPTC' 
					 when p.phasenumber = 'PPPG' then 'PPPG' 
					 when p.phasenumber = 'GVKN' then 'GVKN' 
					 when p.phasenumber = 'VAWJ' then 'VAWJ' 
					 when p.phasenumber = 'HKKN' then 'HKKN'
					 when p.phasenumber = 'HKWJ' then 'HKWJ'
					 when p.phasenumber = 'GVSC' then 'GVSC'
					 when p.phasenumber = 'HKTC' then 'HKTC'
					 when p.phasenumber = 'STKN' then 'STKN'
					 when p.phasenumber = 'STWJ' then 'STWJ'
					 when (j.jobnumber = 'ALTAVW' and  p.phasenumber = 'STTC') then 'STTC'
					 when (j.jobnumber = 'ALTAVW' and  p.phasenumber = 'GVSL') then 'GVSL'
					 END
			else 'TRK HAUL' end Class,
			d.Name Department,
			j.JobNumber,
			p.PhaseNumber, 
			c.CategoryNumber, 
			sum(isnull(dbo.GetTimeSpanHours(lte.StartTime, lte.EndTime), 0)) LoadHours,
			lt.Tons Units, 
			t.[Day], 
			lt.TruckID, 
			lt.TrailerID, 
			lt.PupID, 
			'R' EquipmentCode, 
			lt.LoadTimerID
	from	LoadTimers lt
	left join LoadTimerEntries lte on lt.LoadTimerID = lte.LoadTimerID
	join	Timesheets t on lt.timesheetid = t.timesheetID
	join	Employees e on t.employeeid = e.employeeid
	left join	Jobs j on lt.jobid = j.jobid 
	left join	Phases p on lt.phaseid = p.phaseid 
	left join	Categories c on lt.categoryid = c.categoryid
	join Departments d on t.DepartmentID = d.DepartmentID
	where t.[day] between @startDate and @endDate
    and (t.departmentID in @departmentID)
	and EmployeeNumber != 'Admin'
	group by e.EmployeeNumber, d.Name, j.JobNumber, p.PhaseNumber, c.CategoryNumber, lt.Tons, t.[Day], lt.TruckID, lt.TrailerID, lt.PupID, lt.LoadTimerID
),
LoadDownTimeTimers as
(
	select	e.EmployeeNumber, 
			d.Name Department,
			case when dtr.UseLoadJob=1 then j.JobNumber else isnull(dtr.JobNumber, 'TKOH') end JobNumber,
			case when dtr.UseLoadPhase=1 then p.PhaseNumber else dtr.PhaseNumber end PhaseNumber,
			case when dtr.UseLoadCategory=1 then c.CategoryNumber else dtr.CategoryNumber end CategoryNumber,
			isnull(dbo.GetTimeSpanHours(dtt.StartTime, dtt.StopTime), 0) DownHours,
			t.[day], 
			lt.loadtimerid
	from downtimetimers dtt
	join loadtimers lt on dtt.loadtimerid = lt.loadtimerid
	join timesheets t on lt.timesheetid = t.timesheetid
	join downtimereasons dtr on dtt.downtimereasonid = dtr.downtimereasonid
	join employees e on t.employeeid = e.employeeid
	left join	Jobs j on lt.jobid = j.jobid 
	left join Phases p on lt.phaseid = p.phaseid
	left join Categories c on lt.categoryid = c.categoryid
	join Departments d on t.DepartmentID = d.DepartmentID
	where dtt.loadtimerid is not null
	and t.[day] between @startDate and @endDate
    and (t.departmentID in @departmentID)
	and EmployeeNumber != 'Admin'
),
AdjustedLoadTimes as 
(
	select	EmployeeNumber, Department, Class, JobNumber, PhaseNumber, CategoryNumber, 
			l.LoadHours AdjustedHours,
			Units,
			CONVERT(nvarchar(10),[Day], 101) [Day],
			EquipmentCode, 
			l.LoadHours EquipmentHours,
			l.TruckID, l.TrailerID, l.PupID, l.LoadTimerID
	from	Loads l
	left join ( select sum(ldtt.DownHours) DownHours, LoadTimerID
				from LoadDownTimeTimers ldtt 
				group by ldtt.LoadTimerID) ld on l.LoadTimerID = ld.LoadTimerID
)

select	EmployeeNumber, Department, Class, JobNumber, PhaseNumber, CategoryNumber, AdjustedHours [Hours], Units, [Day], e.EquipmentNumber, EquipmentCode, EquipmentHours
from	AdjustedLoadTimes l
join	Equipment e on l.TruckID = e.EquipmentID
union all
select	EmployeeNumber, Department, Class, JobNumber, PhaseNumber, CategoryNumber, null, null, [Day], e.EquipmentNumber, EquipmentCode, EquipmentHours
from	AdjustedLoadTimes l
join	Equipment e on l.TrailerID = e.EquipmentID
union all
select	EmployeeNumber, Department, Class, JobNumber, PhaseNumber, CategoryNumber, null, null, [Day], e.EquipmentNumber, EquipmentCode, EquipmentHours
from	AdjustedLoadTimes l
join	Equipment e on l.PupID = e.EquipmentID
union all
-- timesheet downtime timers
select e.EmployeeNumber, d.Name Department,  null Class, isnull(dtr.JobNumber, 'TKOH') JobNumber, dtr.PhaseNumber, dtr.CategoryNumber, dbo.GetTimeSpanHours(dtt.StartTime, dtt.StopTime) [Hours],
			null Units, CONVERT(nvarchar(10),t.[Day], 101) [Day], null, null, null
	from downtimetimers dtt
	join timesheets t on dtt.timesheetid = t.timesheetid
	join downtimereasons dtr on dtt.downtimereasonid = dtr.downtimereasonid
	join employees e on t.employeeid = e.employeeid
	join Departments d on d.DepartmentID = t.DepartmentID
	where dtt.loadtimerid is null
	and t.[day] between @startDate and @endDate
    and (t.departmentID in @departmentID)
	and EmployeeNumber != 'Admin'
union all
-- load timer downtime timers
select ld.EmployeeNumber, Department, null Class, ld.JobNumber, ld.PhaseNumber, ld.CategoryNumber, ld.DownHours, null Units, CONVERT(nvarchar(10),ld.[Day], 101) [Day], null, null, null
from LoadDownTimeTimers ld