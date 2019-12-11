declare @PrepDownTimeReasonID int, @PretripDownTimeReasonID int, @PosttripDownTimeReasonID int;
select @PrepDownTimeReasonID = dtr.DowntimeReasonID from downtimereasons dtr where jobnumber = 'TKOH' and PhaseNumber = 'GEN' and CategoryNumber = 'PREP'; 
select @PretripDownTimeReasonID = dtr.DowntimeReasonID from downtimereasons dtr where jobnumber = 'TKOH' and PhaseNumber = 'GEN' and CategoryNumber = 'PRE';
select @PosttripDownTimeReasonID = dtr.DowntimeReasonID from downtimereasons dtr where jobnumber = 'TKOH' and PhaseNumber = 'GEN' and CategoryNumber = 'POST';

with LoadDownTimeTimers as
(
	select	e.EmployeeNumber,
			case when dtr.UseLoadJob=1 then j.JobNumber else isnull(dtr.JobNumber, 'TKOH') end JobNumber,
			case when dtr.UseLoadPhase=1 then p.PhaseNumber else dtr.PhaseNumber end PhaseNumber,
			case when dtr.UseLoadCategory=1 then c.CategoryNumber else dtr.CategoryNumber end CategoryNumber,
			isnull(dbo.GetTimeSpanHours(dtt.StartTime, dtt.StopTime), 0) DownHours,
			t.[day], tr.EquipmentNumber, lt.loadtimerid, e.[Location] Department
	from downtimetimers dtt
	join loadtimers lt on dtt.loadtimerid = lt.loadtimerid
	join timesheets t on lt.timesheetid = t.timesheetid
	join downtimereasons dtr on dtt.downtimereasonid = dtr.downtimereasonid
	join employees e on t.employeeid = e.employeeid
	left join	Jobs j on lt.jobid = j.jobid 
	left join Phases p on lt.phaseid = p.phaseid
	left join Categories c on lt.categoryid = c.categoryid
	left join Equipment tr on lt.TruckID = tr.EquipmentID
	where dtt.loadtimerid is not null
	and t.[day] between @startDate and @endDate
	and (t.departmentID in @departmentID)
	and EmployeeNumber != 'Admin'
)
-- timesheet downtime timers
select e.EmployeeNumber, isnull(dtr.JobNumber, 'TKOH') JobNumber, dtr.PhaseNumber, dtr.CategoryNumber, dbo.GetTimeSpanHours(dtt.StartTime, dtt.StopTime) [Hours], CONVERT(nvarchar(10),t.[Day], 101) [Day], null EquipmentNumber, e.[Location] Department
	from downtimetimers dtt
	join timesheets t on dtt.timesheetid = t.timesheetid
	join downtimereasons dtr on dtt.downtimereasonid = dtr.downtimereasonid
	join employees e on t.employeeid = e.employeeid
	where dtt.loadtimerid is null
	and dtt.DowntimeReasonID not in (@PretripDownTimeReasonID, @PosttripDownTimeReasonID, @PrepDownTimeReasonID)
	and t.[day] between @startDate and @endDate
	and (t.departmentID in @departmentID)
	and EmployeeNumber != 'Admin'
union all
-- load timer downtime timers
select ld.EmployeeNumber, ld.JobNumber, ld.PhaseNumber, ld.CategoryNumber, ld.DownHours, CONVERT(nvarchar(10),ld.[Day], 101) [Day], EquipmentNumber, Department
from LoadDownTimeTimers ld