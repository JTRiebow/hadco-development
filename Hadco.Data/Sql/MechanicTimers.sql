
DECLARE @startOfWeek date, @endOfWeek date, @week date = '2016-02-07', @employeeid int = 842
select @startOfWeek = DATEADD(dd, -(DATEPART(dw, @week)-1), @week) 
select @endOfWeek = DATEADD(dd, 7-(DATEPART(dw, @week)), @week) 

select e.EmployeeNumber
from LoadTimers lt
join Timesheets ts on lt.timesheetid = ts.timesheetid
join Employees e on ts.employeeid = e.employeeid
where ts.Day between @startOfWeek and @endOfWeek

; with timerSummary as
(
	select  e.EmployeeNumber, et.[Day], ltrim(str(sum(convert(decimal(9,2), datediff(mi, ete.ClockIn, ete.ClockOut))/convert(decimal(9,2),60.00)), 5,2)) TotalHours
	from EmployeeTimers et 
	join Employees e on et.EmployeeID = e.EmployeeID
	join EmployeeTimerEntries ete on ete.EmployeeTimerID = et.EmployeeTimerID
	join EmployeeTimecards etc on etc.EmployeeTimecardID = et.EmployeeTimecardID
	where et.Day between @startOfWeek and @endOfWeek
	and ete.clockout is not null
	and etc.ApprovedByAccounting = 1 and etc.ApprovedBySupervisor = 1
	and e.employeeid = @employeeid
	group by e.EmployeeNumber, et.[day]
)
select ts.EmployeeNumber, CONVERT(nvarchar(10),ts.Day, 101) [Day], isnull(ts.TotalHours, 0) TotalHours
from timerSummary ts
order by EmployeeNumber, Day


select e.EmployeeNumber, ts.day Day, datediff(s, jt.StartTime, jt.StopTime), c.jobnumber, c.phasenumber, c.categorynumber
from jobtimers jt
join timesheets ts on jt.timesheetid = ts.timesheetid
join categories c on jt.jobid = c.jobid and jt.phaseid = c.phaseid and jt.categoryid = c.categoryid
join Employees e on ts.EmployeeID = e.EmployeeID
where ts.Day between @startOfWeek and @endOfWeek
and ts.day = '2016-02-10'
	and jt.StopTime is not null
	and ts.departmentid = 5
	and e.employeeid = @employeeid
and datediff(mi, jt.StartTime, jt.StopTime) > 0

select e.EmployeeNumber, ts.day Day, datediff(s, et.StartTime, et.StopTime), eq.EquipmentNumber, est.Name EquipmentServiceType
from equipment eq
join EquipmentTimers et on eq.EquipmentID = et.EquipmentID
join EquipmentServiceTypes est on et.EquipmentServiceTypeID = est.EquipmentServiceTypeID
join timesheets ts on et.timesheetid = ts.timesheetid
join Employees e on ts.EmployeeID = e.EmployeeID
where ts.Day between @startOfWeek and @endOfWeek
and ts.day = '2016-02-10'
	and et.StopTime is not null
	and ts.departmentid = 5
	and e.employeeid = @employeeid
and datediff(mi, et.StartTime, et.StopTime) > 0

---- Mechanic Job Timers
--select e.EmployeeNumber, ts.day Day, ltrim(str(sum(convert(decimal(15,5), datediff(ss, jt.StartTime, jt.StopTime))/convert(decimal(15,5),3600.00)), 15,5)) TotalTime, c.jobnumber, c.phasenumber, c.categorynumber
--from jobtimers jt
--join timesheets ts on jt.timesheetid = ts.timesheetid
--join categories c on jt.jobid = c.jobid and jt.phaseid = c.phaseid and jt.categoryid = c.categoryid
--join Employees e on ts.EmployeeID = e.EmployeeID
--where ts.Day between @startOfWeek and @endOfWeek
--and ts.day = '2016-02-10'
--	and jt.StopTime is not null
--	and ts.departmentid = 5
--	and e.employeeid = @employeeid
--and datediff(mi, jt.StartTime, jt.StopTime) > 0
--group by e.EmployeeNumber, ts.day, c.jobnumber, c.phasenumber, c.categorynumber


-- Mechanic Equipment Timers
--select e.EmployeeNumber, ts.day Day, ltrim(str(sum(convert(decimal(15,5), datediff(ss, et.StartTime, et.StopTime))/convert(decimal(15,5),3600.00)), 15,5)) TotalTime, eq.EquipmentNumber, est.Name EquipmentServiceType
--from equipment eq
--join EquipmentTimers et on eq.EquipmentID = et.EquipmentID
--join EquipmentServiceTypes est on et.EquipmentServiceTypeID = est.EquipmentServiceTypeID
--join timesheets ts on et.timesheetid = ts.timesheetid
--join Employees e on ts.EmployeeID = e.EmployeeID
--where ts.Day between @startOfWeek and @endOfWeek
--and ts.day = '2016-02-10'
--	and et.StopTime is not null
--	and ts.departmentid = 5
--	and e.employeeid = @employeeid
--and datediff(mi, et.StartTime, et.StopTime) > 0
--group by e.EmployeeNumber, ts.day, eq.equipmentnumber, est.Name




