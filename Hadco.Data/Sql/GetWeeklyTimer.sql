select
  etc.employeetimecardid
  , etc.employeeid
  , e.NAME
  , d.NAME Department
  , d.DepartmentID
  , etc.StartOfWeek
  , LEFT(t.supervisor, Len(t.supervisor) - 1) Supervisor
from employeetimecards etc
  join employees e
    on etc.employeeid = e.employeeid
  join departments d
    on etc.subdepartmentid = d.departmentid
  cross apply (
                select s.NAME + ', '
                from employeesupervisors es
                  join employees s
                    on es.supervisorid = s.employeeid
                where es.employeeid = e.employeeid
                for xml path ('')) as t (supervisor)
where etc.employeeTimecardID in @employeeTimecardIDs
order by department,
  NAME;

select
  et.employeetimecardid
  , (DATEDIFF(day, dbo.GetFirstDayOfWeek(da.day), da.day) + 6) % 7 DayNumber
  , da.approvedbysupervisor
  , da.approvedbyaccounting
  , da.approvedbybilling
  , case when EXISTS(
    select *
    from Notes n
      join NoteTypes nt on n.NoteTypeID = nt.NoteTypeID
    where n.Day = da.Day and n.EmployeeID = da.EmployeeID and n.DepartmentID = da.DepartmentID
          and N.Resolved = 0
          and nt.IsSystemGenerated = 1)
  then 1
    else 0 end SystemFlagged
  , case when EXISTS(
    select *
    from Notes n
      join NoteTypes nt on n.NoteTypeID = nt.NoteTypeID
    where n.Day = da.Day and n.EmployeeID = da.EmployeeID and n.DepartmentID = da.DepartmentID
          and N.Resolved = 0
          and nt.IsSystemGenerated = 0)
  then 1
    else 0 end UserFlagged
  , MAX(CAST(et.Injured as int)) Injured
  , max(eto.HasOccurrence) HasOccurrence
  , Sum(Isnull(dbo.Gettimespanhours(ete.clockin, ete.clockout), 0)) TotalHours
from DailyApprovals da
  left join EmployeeTimers et
    on et.Day = da.Day and et.EmployeeID = da.EmployeeID and et.DepartmentID = da.DepartmentID
  left join employeetimerentries ete on ete.employeetimerid = et.employeetimerid
  cross apply (
                select top 1 HasOccurrence
                from (
                       select case when exists(select *
                                               from EmployeTimerOccurrences eto
                                               where et.EmployeeTimerID = eto.EmployeeTimerID)
                         then 1
                              else 0 end HasOccurrence) t
                order by HasOccurrence desc
              ) eto
where et.EmployeeTimecardID in @employeeTimecardIDs
group by et.employeetimecardid,
  da.day,
  et.day,
  et.EmployeeID,
  et.DepartmentID,
  da.EmployeeID,
  da.DepartmentID,
  da.approvedbysupervisor,
  da.approvedbyaccounting,
  da.approvedbybilling