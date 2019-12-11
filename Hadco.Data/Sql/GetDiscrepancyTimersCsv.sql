with EmployeeMinutes as 
(
    select 
        ts.TimesheetID
        , isNull(et.TravelMinutes, 0) TravelMinutes
        , isNull(et.GreaseMinutes, 0) GreaseMinutes
        , isNull(et.DailyMinutes, 0) DailyMinutes
        , isNull(et.ShopMinutes, 0) ShopMinutes
        , Sum(DATEDIFF(minute, ete.ClockIn, ete.ClockOut)) EmployeeMinutes
    from EmployeeTimers et
        join EmployeeTimerEntries ete on ete.EmployeeTimerID = et.EmployeeTimerID
        join Timesheets ts on ts.TimesheetID = et.TimesheetID
        join Employees e on e.EmployeeID = ts.EmployeeID
    where
        ts.Day between @startDate and @endDate
    group by ts.TimesheetID, et.TravelMinutes, et.GreaseMinutes
        , et.DailyMinutes, et.ShopMinutes
)
, EquipmentMinutes as
(
    select 
        ts.TimesheetID
        , isNull(
            sum(
                case 
                    when jt.TimesheetID = ts.TimesheetID
                        then isNull(ejet.EquipmentMinutes, 0)
                    else 0 
                end
            )
            + sum(
                case
                    when
                        jt.StartTime is not null and
                        jt.StopTime is not  null
                    then DateDiff(Minute, jt.StartTime, jt.StopTime)
                    else 0 
                end
            )
            + sum(
                case
                    when lte.StartTime is not null
                        and lte.EndTime is not null
                    then DateDiff(Minute, lte.StartTime, lte.EndTime)
                    else 0 
                end
            )
            , 0.0
        ) as EquipmentMinutes
    from Timesheets ts
        left join JobTimers jt on jt.TimesheetID = ts.TimesheetID
        left join EmployeeJobTimers ejt on ejt.JobTimerID = jt.JobTimerID
        left join EmployeeJobEquipmentTimers ejet on ejet.EmployeeJobTimerID = ejt.EmployeeJobTimerID
        left Join LoadTimers lt on lt.TimesheetID = ts.TimesheetID
        left join LoadTimerEntries lte on lte.LoadTimerID = lt.LoadTimerID
        join Employees e on e.EmployeeID = ts.EmployeeID
    where 
        ts.Day between @startDate and @endDate
        and (ts.departmentID in @departmentID)
        and (e.EmployeeNumber != 'Admin' or e.EmployeeNumber is null)
    group by ts.TimesheetID
)
, EquipmentTimersInfo as
(
    select 
        ts.TimesheetID
        , sum(
            case
                when ete.StartTime is not null
                    and ete.StopTime is not null
                    then isNull(DateDiff(minute, ete.StartTime, ete.StopTime), 0)
                else 0
            end
        ) EquipmentTimerMinutes
    from Timesheets ts
        left join EquipmentTimers et on et.TimesheetID = ts.TimesheetID
        left join EquipmentTimerEntries ete on ete.EquipmentTimerID = et.EquipmentTimerID
    group by ts.TimesheetID
)
, AllocationMinutes as 
(
    select 
        ts.TimesheetID
        , sum(isNull(ejt.LaborMinutes, 0))
            + eti.EquipmentTimerMinutes
             LaborMinutes
        , em.EquipmentMinutes
        , e.name
        , ts.day
    from Timesheets ts
        left join JobTimers jt on jt.TimesheetID = ts.TimesheetID
        left join EmployeeJobTimers ejt on ejt.JobTimerID = jt.JobTimerID
        left join EquipmentMinutes em on em.TimesheetID = ts.TimesheetID
        left join EquipmentTimersInfo eti on eti.TimesheetID = ts.TimesheetID
        join Employees e on e.EmployeeID = ts.EmployeeID
    where
        ts.Day between @startDate and @endDate
    group by ts.TimesheetID, em.EquipmentMinutes, eti.EquipmentTimerMinutes
    , e.name, ts.day
)
, DowntimeTimersInfo as 
(
    select 
        ts.TimesheetID
        , sum(isNull(DateDiff(minute, dt.StartTime, dt.StopTime), 0)) DowntimeMinutes
    from Timesheets ts
        left join DowntimeTimers dt on dt.TimesheetID = ts.TimesheetID
    where
        ts.Day between @startDate and @endDate
    group by ts.TimesheetID
)
, AllTimers as (
select 
    e.[Name],
    isnull(e.EmployeeNumber, e.Username) Employee,
    CONVERT(nvarchar(10),ts.Day, 101) Day,
    em.EmployeeMinutes / 60.0 EmployeeHours,
    (am.LaborMinutes + am.EquipmentMinutes + em.TravelMinutes + em.GreaseMinutes + em.DailyMinutes + em.ShopMinutes + dti.DowntimeMinutes) / 60.0 TotalHours,
    d.[Name] Department,
    em.TravelMinutes / 60.0 TravelHours,
    em.GreaseMinutes / 60.0 GreaseHours,
    em.DailyMinutes / 60.0 DailyHours,
    em.ShopMinutes / 60.0 ShopHours,
    am.LaborMinutes / 60.0 LaborHours,
    am.EquipmentMinutes / 60.0 EquipmentHours,
    dti.DowntimeMinutes / 60.0 DowntimeHours
from Timesheets ts
    join EmployeeMinutes em on em.TimesheetID = ts.TimesheetID
    join AllocationMinutes am on am.TimesheetID = ts.TimesheetID
    join DowntimeTimersInfo dti on dti.TimesheetID = ts.TimesheetID
    join Employees e on ts.EmployeeID = e.EmployeeID
    join Departments d on d.DepartmentID = ts.DepartmentID
where 
    ts.Day between @startDate and @endDate
    and (ts.departmentID in @departmentID)
    and (e.EmployeeNumber != 'Admin' or e.EmployeeNumber is null)
group by e.EmployeeNumber, e.Username, e.Name, ts.day
    , em.TravelMinutes, em.GreaseMinutes, em.DailyMinutes, em.ShopMinutes
    , am.LaborMinutes, am.EquipmentMinutes, em.EmployeeMinutes, dti.DowntimeMinutes
    , d.Name
)
select 
    [Name],
    Employee,
    Day,
    EmployeeHours,
    TotalHours,
    Department,
    TravelHours,
    GreaseHours,
    DailyHours,
    ShopHours,
    LaborHours,
    EquipmentHours,
    DowntimeHours,
    EmployeeHours - TotalHours HoursDifference
from AllTimers
    where (TotalHours > 0 or EmployeeHours > 0)
    and EmployeeHours != TotalHours
order by Name, Day