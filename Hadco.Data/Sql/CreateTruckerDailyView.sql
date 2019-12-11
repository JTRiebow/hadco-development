CREATE VIEW TruckerDailies
AS
select	convert(int, ROW_NUMBER() over (order by [Date])) TruckerDailyID,
		[Date], 
         Name, 
         LoadTimerID, 
         TicketNumber,
		 DowntimeTimerID,
         DowntimeCode, 
         Truck, 
         Trailer, 
         PupTrailer, 
         Tons,
         LoadSite,
         DumpSite,
         BillType, 
         Note, 
		 InvoiceNumber, 
         Material, 
         LoadEquipment,
         Job,
         JobID, 
         Phase,
         PhaseID, 
         Category,
         CategoryID,  
         [Location],
         DepartmentID,
         TotalHours,
 	     CalculatedRevenue,
		 PricePerUnit
from (
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
 	   case when lt.BillTypeID = 1 then lte1.TotalHours * lt.PricePerUnit 
			when lt.BillTypeID = 2 then lt.Tons * lt.PricePerUnit 
			when lt.BillTypeID = 3 then lt.PricePerUnit 
		end as CalculatedRevenue,
	   lt.PricePerUnit
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
 cross apply (select lte.LoadTimerID, convert(decimal(8, 2),
                                         sum(isnull(dbo.GetTimeSpanHours(lte.StartTime, lte.EndTime), 0))
										 -sum(isnull(dbo.GetTimeSpanHours(d.StartTime, d.StopTime), 0))) TotalHours
             from LoadTimerEntries lte
             left
             join dbo.DowntimeTimers d on lte.LoadTimerID = d.LoadTimerID
             where lte.LoadTimerID = lt.LoadTimerID
             group by lte.loadTimerID
             ) lte1
 union all
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
 ) td