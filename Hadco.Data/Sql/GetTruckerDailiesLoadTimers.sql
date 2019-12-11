 select t.Day [Date], 
         e.Name, 
         lt.LoadTimerID, 
         lt.TicketNumber,
		 null DowntimeTimerID,
         null DowntimeCode, 
         tr.EquipmentNumber as Truck, 
         tlr.EquipmentNumber as Trailer, 
         pp.EquipmentNumber as PupTrailer, 
         cast(lt.Tons as varchar) Tons,
         lt.StartLocation LoadSite,
         lt.EndLocation DumpSite,
         b.Name as BillType, 
         lt.Note, lt.InvoiceNumber, 
         m.Name as Material, 
         le.EquipmentNumber as LoadEquipment,
         j.JobNumber as Job,
         j.JobID, 
         ph.PhaseNumber as Phase,
         ph.PhaseID, 
         c.CategoryNumber as Category,
         c.CategoryID,  
         e.[Location],
         t.DepartmentID,
        lte1.TotalHours,
 	   convert(decimal(8, 2), case when lt.BillTypeID = 1 then lte1.TotalHours * lt.PricePerUnit 
			when lt.BillTypeID = 2 then lt.Tons * lt.PricePerUnit 
			when lt.BillTypeID = 3 then lt.PricePerUnit 
		end) as CalculatedRevenue,
	   convert(decimal(8, 2), lt.PricePerUnit) PricePerUnit
 from dbo.LoadTimers lt
 join dbo.Timesheets t on lt.TimesheetID = t.TimesheetID
 join dbo.Employees e on t.EmployeeID = e.EmployeeID
 left join dbo.Equipment tr on lt.TruckID = tr.EquipmentID
 left join dbo.Equipment tlr on lt.TrailerID = tlr.EquipmentID
 left join dbo.Equipment pp on lt.PupID = pp.EquipmentID
 left join dbo.Equipment le on lt.LoadEquipmentID = le.EquipmentID
 left join dbo.Materials m on lt.MaterialID = m.MaterialID
 left join dbo.Jobs j on lt.JobID = j.JobID
 left join dbo.Phases ph on lt.PhaseID = ph.PhaseID
 left join dbo.Categories c on lt.CategoryID = c.CategoryID
 left join dbo.BillTypes b on lt.BillTypeID = b.BillTypeID
 cross apply (select lte.LoadTimerID, convert(decimal(8, 2), sum(isnull(dbo.GetTimeSpanHours(lte.StartTime, lte.EndTime), 0))) TotalHours
             from LoadTimerEntries lte
             left
             join dbo.DowntimeTimers d on lte.LoadTimerID = d.LoadTimerID
             where lte.LoadTimerID = lt.LoadTimerID
             group by lte.loadTimerID
             ) lte1