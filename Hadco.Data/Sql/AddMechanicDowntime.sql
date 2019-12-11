declare @downTimeCategoryID int, @downTimeJobID int, @downTimePhaseID int;

select @downTimeCategoryID = c.CategoryID, @downTimeJobID = c.JobID, @downTimePhaseID = c.PhaseID
from Categories c
where c.CategoryNumber ='Down' and c.JobNumber = 'HADCO' and c.PhaseNumber = 'Shop'; -- this is a special downtime job for mechanics downtime

delete jt
from JobTimers jt
where jt.CategoryID = @downTimeCategoryID
and jt.timesheetid = @timesheetid;

with EmployeeTimersCte as
(
	select sum(DateDiff(minute, ete.ClockIn, ete.ClockOut)) TotalMinutes, t.TimesheetID
    from Timesheets t
    join EmployeeTimers et on t.TimesheetID = et.TimesheetID
    join EmployeeTimerEntries ete on et.EmployeeTimerID = ete.EmployeeTimerID
    where ete.ClockOut is not null
    and ete.ClockIn > '2000-01-01' -- some dates are in the system that are way old and cause an overflow on the datediff.
    and t.TimeSheetID = @timesheetid
    group by t.TimesheetID
)

, JobAndEquipmentTimers as
(
    select sum(TotalMinutes) TotalMinutes, TimesheetID 
	from (
		select sum(DateDiff(minute, jt.StartTime, jt.StopTime)) TotalMinutes, jt.TimesheetID
        from JobTimers jt
        where jt.StopTime is not null
        and jt.StartTime is not null
        and jt.CategoryID != @downTimeCategoryID
        and jt.TimeSheetID = @timesheetid
        group by jt.TimesheetID
        union
		select sum(DateDiff(Minute, ete.StartTime, ete.StopTime)), et.TimesheetID
        from EquipmentTimers et
        join EquipmentTimerEntries ete on et.EquipmentTimerID = ete.EquipmentTimerID 
        where ete.StartTime is not null
        and ete.StopTime is not null
        and et.TimeSheetID = @timesheetid
        group by et.TimesheetID) t
    group by t.TimesheetID
)


insert into JobTimers (StartTime, StopTime, JobID, PhaseID, CategoryID, TimesheetID, Diary)
select  dateadd(hh, 7, convert(datetimeoffset, t.Day)) StartTime, 
        dateadd(minute, (etc.TotalMinutes - jet.TotalMinutes), dateadd(hh, 7, convert(datetimeoffset,t.Day))) StopTime, 
        @downTimeJobID JobID, @downTimePhaseID PhaseID, @downTimeCategoryID CategoryID, t.TimesheetID, 'System Produced Downtime'
from EmployeeTimersCte etc
join JobAndEquipmentTimers jet on etc.TimesheetID = jet.TimesheetID
join Timesheets t on jet.TimesheetID = t.TimesheetID
join Employees e on t.EmployeeID = e.EmployeeID
where (etc.TotalMinutes - jet.TotalMinutes) > 0
and t.TimeSheetID = @timesheetid
