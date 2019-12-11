declare @PrepDownTimeReasonID int, @PretripDownTimeReasonID int, @PosttripDownTimeReasonID int

select @PrepDownTimeReasonID = dtr.DowntimeReasonID from downtimereasons dtr where jobnumber = 'TKOH' and PhaseNumber = 'GEN' and CategoryNumber = 'PREP'; 
select @PretripDownTimeReasonID = dtr.DowntimeReasonID from downtimereasons dtr where jobnumber = 'TKOH' and PhaseNumber = 'GEN' and CategoryNumber = 'PRE';
select @PosttripDownTimeReasonID = dtr.DowntimeReasonID from downtimereasons dtr where jobnumber = 'TKOH' and PhaseNumber = 'GEN' and CategoryNumber = 'POST';

 --delete all existing prep, pretrip and posttrip downtime timers

	delete dt
	from DowntimeTimers dt
	join Timesheets t on dt.timesheetid = t.timesheetid
	where dt.SystemGenerated = 1
	and t.timesheetid = @TimesheetID;
	
-- First Calculate the pretrip and posttrip downtime timers.
with EmployeeTimersCte as
(
	select min(ete.ClockIn) FirstClockIn, min(ete.ClockOut) FirstClockOut, max(ete.ClockIn) LastClockIn, max(ete.ClockOut) LastClockOut, t.TimesheetID
	from Timesheets t
	join EmployeeTimers et on t.TimesheetID = et.TimesheetID
	join EmployeeTimerEntries ete on et.EmployeeTimerID = ete.EmployeeTimerID
	where ete.ClockOut is not null
	and ete.ClockIn > '2000-01-01' -- some dates are in the system that are way old and cause an overflow on the datediff.
	and t.TimeSheetID = @timesheetid
	group by t.TimesheetID
), LoadTimersCte as
(
	select min(StartTime) FirstLoadOrDowntimeStart, max(EndTime) LastLoadOrDowntimeEnd, t.timesheetid
	from (
		select StartTime, EndTime, lt.TimesheetID
		from LoadTimers lt
		left join LoadTimerEntries lte on lt.LoadTimerID = lte.LoadTimerID 
		where lte.StartTime is not null
		and lte.EndTime is not null
		and lt.timesheetid = @timesheetid
		union all
		select StartTime, StopTime, TimeSheetID
		from DowntimeTimers dts
		where timesheetid = @TimesheetID
	) t
	group by t.timesheetid
)
--Pre/PostTrip insert
insert downtimeTimers (DowntimeReasonID, StartTime, StopTime, TimesheetID, SystemGenerated)  		 
select 	@PretripDownTimeReasonID,
		etc.FirstClockIn StartTime,
		case when etc.FirstClockOut < ltc.FirstLoadOrDowntimeStart then etc.FirstClockOut else ltc.FirstLoadOrDowntimeStart end StopTime,
		etc.timesheetID,
		1
from 	EmployeeTimersCte etc 
join	LoadTimersCte ltc on etc.timesheetid = ltc.timesheetid
join	Timesheets t on etc.timesheetid = t.timesheetid
where etc.FirstClockIn < ltc.FirstLoadOrDowntimeStart
and t.timesheetid = @timesheetid
union 
select 	@PosttripDownTimeReasonID,
		case when ltc.LastLoadOrDowntimeEnd < etc.LastClockIn then etc.LastClockIn else ltc.LastLoadOrDowntimeEnd end StartTime,
		etc.LastClockOut StopTime, 
		etc.timesheetID,
		1
from 	EmployeeTimersCte etc 
join	LoadTimersCte ltc on etc.timesheetid = ltc.timesheetid
join	Timesheets t on etc.timesheetid = t.timesheetid
where ltc.LastLoadOrDowntimeEnd < etc.LastClockOut
and t.timesheetid = @timesheetid;

--PrepTime insert
with EmployeeTimersCte as
(
	select sum(dbo.GetTimespanHours(ete.ClockIn, ete.ClockOut)) TotalHours, t.TimesheetID
	from Timesheets t
	join EmployeeTimers et on t.TimesheetID = et.TimesheetID
	join EmployeeTimerEntries ete on et.EmployeeTimerID = ete.EmployeeTimerID
	where ete.ClockOut is not null
	and ete.ClockIn > '2000-01-01' -- some dates are in the system that are way old and cause an overflow on the datediff.
	and t.TimeSheetID = @timesheetid
	group by t.TimesheetID
),
LoadAndDowntimeTimers as
(
	select sum(TotalHours) TotalHours, TimesheetID 
	from (select sum(dbo.GetTimespanHours(lte.StartTime, lte.EndTime)) TotalHours, lt.TimesheetID
		from LoadTimers lt
		left join LoadTimerEntries lte on lt.LoadTimerID = lte.LoadTimerID
		where lte.StartTime is not null
		and lte.EndTime is not null
		and lt.TimeSheetID = @timesheetid
		group by lt.TimesheetID
		--union all
		--select -1.00*sum(dbo.GetTimespanHours(bt.StartTime, bt.StopTime)) TotalHours, lt.TimesheetID
		--from BreakTimers bt
		--join LoadTimers lt on bt.LoadTimerID = lt.LoadTimerID
		--where bt.StartTime is not null
		--and bt.StopTime is not null
		--and lt.TimeSheetID = @timesheetid
		--group by lt.TimesheetID
		union all
		select sum(dbo.GetTimespanHours(dt.StartTime, dt.StopTime)), dt.TimesheetID
		from DowntimeTimers dt 
		where dt.StartTime is not null
		and dt.StopTime is not null
		and dt.TimesheetID is not null
		and dt.TimeSheetID = @timesheetid
		group by dt.TimesheetID) t
	group by t.TimesheetID
)
insert into DowntimeTimers (DowntimeReasonID, StartTime, StopTime, TimesheetID, SystemGenerated)
select @PrepDownTimeReasonID, 
		dateadd(hh, 7, convert(datetimeoffset, t.Day)) StartTime, 
		dateadd(millisecond, ((etc.TotalHours - lat.TotalHours)*3600*1000), dateadd(hh, 7, convert(datetimeoffset, t.Day))) StopTime, 
		t.TimesheetID,
		1
from EmployeeTimersCte etc
join LoadAndDowntimeTimers lat on etc.TimesheetID = lat.TimesheetID
join Timesheets t on lat.TimesheetID = t.TimesheetID
join Employees e on t.EmployeeID = e.EmployeeID
where ((etc.TotalHours - lat.TotalHours)*3600) > 2 -- if the seconds are greater than 2 then insert a prep downtime timer.
and t.TimeSheetID = @timesheetid;
