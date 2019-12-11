with QuantitiesCTE as
(
	select distinct 
		et.[Day]
		, c.JobNumber
		, c.PhaseNumber
		, c.CategoryNumber
		, c.UnitsOfMeasure
		, c.PlannedQuantity Est
		, jt.NewQuantity
		, jt.JobTimerID
	from EmployeeTimers et
		join Timesheets t on t.TimesheetID = et.TimesheetID
		join JobTimers jt on jt.TimesheetID = t.TimesheetID
		left join EmployeeJobTimers ejt on et.EmployeeTimerID = ejt.EmployeeTimerID and jt.JobTimerID = ejt.JobTimerID
		join Categories c on jt.CategoryID = c.CategoryID
	where
		(@startDate <= et.[Day]
			or @startDate is null)
		and (et.[Day] <= @endDate
			or @endDate is null)
		and et.DepartmentID in @departmentID
)

, PreviousQuantityCTE as
(
	select
		jti.JobTimerID
		, sum(jti2.NewQuantity) PreviousQuantity
	from JobTimers jti
		join Timesheets t on jti.TimesheetID = t.TimesheetID
		join JobTimers jti2 on jti2.CategoryID = jti.CategoryID
		join Timesheets t2 on jti2.TimesheetID = t2.TimesheetID
		join QuantitiesCTE q on q.JobTimerID = jti.JobTimerID
	group by jti.JobTimerID
)

, OtherNewQuantitiesCTE as
(
	select 
		jti.JobTimerID
		, sum(jti2.NewQuantity) OtherNewQuantity
	from JobTimers jti
		join Timesheets t on jti.TimesheetID = t.TimesheetID
		join JobTimers jti2 on jti2.CategoryID = jti.CategoryID
		join Timesheets t2 on jti2.TimesheetID = t2.TimesheetID and t.Day = t2.Day and t.TimesheetID != t2.TimesheetID
		join QuantitiesCTE q on q.JobTimerID = jti.JobTimerID
	group by jti.JobTimerID
)

select
	q.*
	, pq.PreviousQuantity
	, onq.OtherNewQuantity
	, isNull(pq.PreviousQuantity, 0) + isNull(q.NewQuantity, 0) + isNull(onq.OtherNewQuantity, 0) [Total]
from QuantitiesCTE q
	left join PreviousQuantityCTE pq on pq.JobTimerID = q.JobTimerID
	left join OtherNewQuantitiesCTE onq on onq.JobTimerID = q.JobTimerID
where (q.NewQuantity is not null
	or pq.PreviousQuantity is not null
	or onq.OtherNewQuantity is not null)
order by [Day];