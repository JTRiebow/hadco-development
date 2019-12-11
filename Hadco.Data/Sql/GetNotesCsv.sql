select 
	e.[Name]
	, n.[Description]
	, nt.[Name] NoteType
	, nt.[Description] NoteTypeDescription
	, Resolved
from Notes n
	left join Employees e on e.EmployeeID = n.EmployeeID
	left join NoteTypes nt on nt.NoteTypeID = n.NoteTypeID
where
	(@startDate <= n.[Day]
		or @startDate is null)
	and (n.[Day] <= @endDate
		or @endDate is null)
	and (n.DepartmentID in @departmentID)