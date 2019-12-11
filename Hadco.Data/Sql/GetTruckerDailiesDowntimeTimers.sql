select t.Day [Date],
 		e.Name, 
 		lt.LoadTimerID, 
 		null, 
		d.DowntimeTimerID,
 		dr.Code DowntimeCode,
         tr.EquipmentNumber, 
 		trl.EquipmentNumber, 
 		pp.EquipmentNumber, 
 		null, null, null, null, null, null, null, null, 
 		case when dr.UseLoadJob = 1 then j.JobNumber else dr.JobNumber end JobNumber, null,
 		case when dr.UseLoadPhase = 1 then ph.PhaseNumber else dr.PhaseNumber end PhaseNumber, null, 
 		case when dr.UseLoadCategory = 1 then c.CategoryNumber else dr.CategoryNumber end CategoryNumber, null, 
 		e.[Location], 
 		t.DepartmentID, 
 		convert(decimal(8, 2), dbo.GetTimeSpanHours(d.StartTime, d.StopTime)), 
 		null, null
 from LoadTimers lt
 join Timesheets t on lt.TimesheetID = t.TimesheetID
 join Employees e on t.EmployeeID = e.EmployeeID
 join DowntimeTimers d on lt.LoadTimerID = d.LoadTimerID
 join DowntimeReasons dr on d.DowntimeReasonID = dr.DowntimeReasonID
 join Equipment tr on lt.TruckID = tr.EquipmentID
 left join Equipment trl on lt.TrailerID = trl.EquipmentID
 left join Equipment pp on lt.PupID = pp.EquipmentID
 left join dbo.Jobs j on lt.JobID = j.JobID
 left join dbo.Phases ph on lt.PhaseID = ph.PhaseID
 left join dbo.Categories c on lt.CategoryID = c.CategoryID

