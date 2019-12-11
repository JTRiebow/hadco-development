--STRING_AGG requires SQL server 2017 or higher (or Azure Sql). 
--This may not work on everyone's local environment
select distinct
  CONVERT(nvarchar(10), et.Day, 101) Date
  , e.Name
  , o.Name Occurrence
  , d.Name Department
  , (select STRING_AGG(Description, ' , ')
      from Notes n
      where NoteTypeID = 10 -- Occurrence type
            and n.EmployeeID = et.EmployeeID
            and n.DAY = et.Day
            and n.DepartmentID = et.DepartmentID) Notes
from Occurrences o
  join EmployeTimerOccurrences eto on o.OccurrenceID = eto.OccurrenceID
  join EmployeeTimers et on eto.EmployeeTimerID = et.EmployeeTimerID
  join Employees e on et.EmployeeID = e.EmployeeID
  join Departments d on et.DepartmentID = d.DepartmentID
where et.Day between @startDate and @endDate
and (et.departmentID in @departmentID)