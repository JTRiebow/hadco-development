SELECT 
	lt.LoadTimerID,
	b.Name as BillType, 
	CONVERT(nvarchar(10),t.Day, 126) as [Date], 
	e.Name,
	tk.EquipmentNumber as Truck, 
	tr.EquipmentNumber as Trailer, 
	p.EquipmentNumber as PupTrailer,
	lt.TicketNumber,  
	lt.Tons,
	lt.StartLocation as LoadSite, 
	lt.EndLocation as DumpSite, 
	m.Name as Material, 
	le.EquipmentNumber as LoadEquipment,
	j.JobNumber as Job,
	j.JobID, 
	ph.PhaseNumber as Phase,
	ph.PhaseID, 
	c.CategoryNumber as Category,
	c.CategoryID, 
	CAST(CAST(SUM(ISNULL(DATEDIFF(MINUTE, lte.StartTime, lte.EndTime), 0)) 
	        - SUM(ISNULL(DATEDIFF(MINUTE, d.StartTime, d.StopTime), 0)) as decimal)/60 as decimal(8, 2)) as TotalHours,
	lt.InvoiceNumber,
	lt.Note,
	e.[Location]
FROM LoadTimers as lt
	LEFT JOIN LoadTimerEntries as lte on lt.LoadTimerID = lte.LoadTimerID
	JOIN Timesheets as t on lt.TimesheetID = t.TimesheetID
	JOIN Employees as e on t.EmployeeID = e.EmployeeID
	JOIN Equipment as tk on lt.TruckID = tk.EquipmentID
	LEFT JOIN Equipment as tr on lt.TrailerID = tr.EquipmentID
	LEFT JOIN Equipment as p on lt.PupID = p.EquipmentID
	LEFT JOIN Equipment as le on lt.LoadEquipmentID = le.EquipmentID
	LEFT JOIN Materials as m on lt.MaterialID = m.MaterialID
	LEFT JOIN Jobs as j on lt.JobID = j.JobID
	LEFT JOIN Phases as ph on lt.PhaseID = ph.PhaseID
	LEFT JOIN Categories as c on lt.CategoryID = c.CategoryID
	LEFT JOIN BillTypes as b on lt.BillTypeID = b.BillTypeID
	LEFT JOIN DowntimeTimers as d on lt.LoadTimerID = d.LoadTimerID
WHERE lt.LoadTimerID = @loadTimerID
GROUP BY t.[Day],
		e.Name, lt.LoadTimerID, lt.TicketNumber, 
        tk.EquipmentNumber, 
		tr.EquipmentNumber, 
		p.EquipmentNumber, 
        lt.Tons, lt.StartLocation, 
		lt.EndLocation, 
		b.Name, 
		lt.Note, lt.InvoiceNumber, 
		m.Name, 
		le.EquipmentNumber,
        j.JobNumber, j.JobID, 
		ph.PhaseNumber, ph.PhaseID, 
		c.CategoryNumber, c.CategoryID, 
		j.JobNumber, 
		e.[location]
